﻿using Conditions;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace DDD.Core.Infrastructure.Serialization
{
    using DDD.Serialization;

    public class DataContractSerializerWrapper : IXmlSerializer
    {

        #region Fields

        private readonly XmlReaderSettings readerSettings;
        private readonly XmlWriterSettings writerSettings;

        #endregion Fields

        #region Constructors

        public DataContractSerializerWrapper()
        {
            this.writerSettings = new XmlWriterSettings
            {
                Encoding = XmlSerializationOptions.Encoding,
                Indent = XmlSerializationOptions.Indent
            };
            this.readerSettings = new XmlReaderSettings();
        }

        private DataContractSerializerWrapper(XmlWriterSettings writerSettings,
                                              XmlReaderSettings readerSettings)
        {
            Condition.Requires(writerSettings, nameof(writerSettings)).IsNotNull();
            Condition.Requires(readerSettings, nameof(readerSettings)).IsNotNull();
            this.writerSettings = writerSettings;
            this.readerSettings = readerSettings;
        }

        #endregion Constructors

        #region Properties

        public Encoding Encoding => this.writerSettings.Encoding;

        public bool Indent => this.writerSettings.Indent;

        #endregion Properties

        #region Methods

        public static DataContractSerializerWrapper Create(XmlWriterSettings writerSettings,
                                                           XmlReaderSettings readerSettings)
        {
            return new DataContractSerializerWrapper(writerSettings, readerSettings);
        }

        public static DataContractSerializerWrapper Create(Encoding encoding, bool indent = true)
        {
            Condition.Requires(encoding, nameof(encoding)).IsNotNull();
            var writerSettings = new XmlWriterSettings
            {
                Encoding = encoding,
                Indent = indent
            };
            var readerSettings = new XmlReaderSettings();
            return new DataContractSerializerWrapper(writerSettings, readerSettings);
        }

        public T Deserialize<T>(Stream stream)
        {
            Condition.Requires(stream, nameof(stream)).IsNotNull();
            using (var reader = XmlReader.Create(stream, this.readerSettings))
            {
                var serializer = new DataContractSerializer(typeof(T));
                try
                {
                    return (T)serializer.ReadObject(reader);
                }
                catch(System.Runtime.Serialization.SerializationException exception)
                {
                    throw new SerializationException(typeof(T), exception);
                }
            }
        }

        public void Serialize(Stream stream, object obj)
        {
            Condition.Requires(stream, nameof(stream)).IsNotNull();
            Condition.Requires(obj, nameof(obj)).IsNotNull();
            using (var writer = XmlWriter.Create(stream, this.writerSettings))
            {
                var serializer = new DataContractSerializer(obj.GetType());
                try
                {
                    serializer.WriteObject(writer, obj);
                }
                catch (System.Runtime.Serialization.SerializationException exception)
                {
                    throw new SerializationException(obj.GetType(), exception);
                }
            }
        }

        #endregion Methods

    }
}