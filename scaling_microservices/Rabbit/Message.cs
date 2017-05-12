using System;
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
        public IBasicProperties Properties { get; set; }
        public byte[] body { get; set; }

        public string StringBody
        {
            get
            {
                if(Properties.ContentEncoding == QueueRequest.classname)
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new QueueRequest(body));
                }
                switch (Properties.ContentEncoding)
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
                if (Properties != null)
                {
                    Properties.ContentEncoding = "UTF8";
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
                return Properties.ContentEncoding;
            }
            set
            {
                Properties.ContentEncoding = value;
            }
        }

        public string CorrelationId
        {
            get
            {
                return Properties.CorrelationId;
            }
        }

        public Message()
        {
            this.Properties = new BasicProperties();
        }

        public void SetQueueRequest(QueueRequest req)
        {
            this.Encoding = QueueRequest.classname;
            this.body = req.ToByteArray();
        }
    }
}
