﻿using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace scaling_microservices.Rabbit
{
    /// <summary>
    /// not safe, when using a new message object with no properties.
    /// use endpoint.Message instead
    /// </summary>
    public class Message
    {
        public IBasicProperties properties { get; set; }
        public byte[] body { get; set; }

        public string StringBody
        {
            get
            {
                if(properties.ContentEncoding == QueueRequest.classname)
                {
                    //TODO: either deserialize or throw
                    return "QueueRequest";
                }
                switch (properties.ContentEncoding)
                {
                    case "UTF8":
                        {
                            return System.Text.Encoding.UTF8.GetString(body);
                        }
                    case "ASCII":
                        {
                            return System.Text.Encoding.ASCII.GetString(body);
                        }
                    case "UTF32":
                        {
                            return System.Text.Encoding.UTF32.GetString(body);
                        }
                    case "Unicode":
                        {
                            return System.Text.Encoding.Unicode.GetString(body);
                        }
                    default:
                        throw new Exception("Invalid Encoding");
                }
            }
            set
            {
                if (properties != null)
                {
                    properties.ContentEncoding = "UTF8";
                    body = System.Text.Encoding.UTF8.GetBytes(value);
                }
                else
                {
                    throw new NullReferenceException("properties of Message object were not set");
                }
            }
        }

        public string Encoding
        {
            get
            {
                return properties.ContentEncoding;
            }
        }

        public string CorrelationId
        {
            get
            {
                return properties.CorrelationId;
            }
        }

        public Message()
        {
            this.properties = new BasicProperties();
        }
    }
}
