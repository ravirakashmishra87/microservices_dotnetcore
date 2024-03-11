namespace Services.CouponAPI.Models.DTO
{
    public class CouponDto
    {
        public int CouponID { get; set; }
        public string CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public int MinimumAmount { get; set; }
    }
}
