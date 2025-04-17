using System.Runtime.Serialization;
using GerenciadorPedidos.Infra.CrossCutting.Logs;

namespace GerenciadorPedidos.Infra.CrossCutting.Utilities.Error
{
    [Serializable]
    public class BusinessException : Exception
    {
        public BusinessException(string message)
            : base(message)
        {
            Log.Error(this, message);
        }

        public BusinessException(string message, Exception ex)
            : base(message, ex)
        {
            Log.Error(this, message);
        }

        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            var errorMessage = (string)info.GetValue("ErrorMessage", typeof(string));
            Log.Error(errorMessage);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            base.GetObjectData(info, context);

            info.AddValue("ErrorMessage", Message);
        }
    }
}
