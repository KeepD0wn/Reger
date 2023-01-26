    public class RootAccount
    {
        public int id { get; set; }
        public string email { get; set; }
        public double balance { get; set; }
        public int rating { get; set; }
        public DefaultCountry default_country { get; set; }
        public DefaultOperator default_operator { get; set; }
        public double frozen_balance { get; set; }
    }
