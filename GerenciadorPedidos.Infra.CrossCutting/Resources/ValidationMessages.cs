namespace GerenciadorPedidos.Infra.CrossCutting.Resources
{
    public static class ValidationMessages
    {
        public const string Required = "O campo {0} é obrigatório.";
        public const string InvalidFormat = "O formato do campo {0} é inválido.";
        public const string MinLength = "O campo {0} deve ter no mínimo {1} caracteres.";
        public const string MaxLength = "O campo {0} deve ter no máximo {1} caracteres.";
        public const string Range = "O campo {0} deve estar entre {1} e {2}.";
        public const string Email = "O campo {0} deve ser um endereço de e-mail válido.";
        public const string PhoneNumber = "O campo {0} deve ser um número de telefone válido.";
        public const string Url = "O campo {0} deve ser uma URL válida.";
        public const string CreditCard = "O campo {0} deve ser um número de cartão de crédito válido.";
        public const string Date = "O campo {0} deve ser uma data válida.";
        public const string Time = "O campo {0} deve ser um horário válido.";
        public const string DateTime = "O campo {0} deve ser uma data e hora válidas.";
        public const string Guid = "O campo {0} deve ser um GUID válido.";
        public const string Json = "O campo {0} deve ser um JSON válido.";
        public const string Xml = "O campo {0} deve ser um XML válido.";
        public const string Base64 = "O campo {0} deve ser uma string Base64 válida.";
        public const string Regex = "O campo {0} não corresponde ao padrão esperado.";
        public const string Compare = "Os campos {0} e {1} não são iguais.";
        public const string Remote = "O valor do campo {0} já está em uso.";
        public const string Custom = "O campo {0} não é válido.";
    }
} 