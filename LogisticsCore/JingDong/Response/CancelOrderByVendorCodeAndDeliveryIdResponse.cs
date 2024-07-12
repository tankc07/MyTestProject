namespace LogisticsCore.JingDong.Response
{
    public class CancelOrderByVendorCodeAndDeliveryIdResponse : FreshMedicineDeliveryResponseBase
    {
        public string data { get; set; }
        public bool HasData => data != null;
    }
}
