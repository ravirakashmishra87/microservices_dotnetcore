﻿namespace MS_Web.Utility
{
    public class SD
    {
        public static string? CouponAPIBase { get; set; }
        public static string? AuthAPIBase {  get; set; }
        public static string? ProductAPIBase {  get; set; }
        public static string? CartAPIBase {  get; set; }

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookieName = "JWTToken";
        public enum ApiType
        {
            GET,
            POST, 
            PUT, 
            DELETE

        }
    }
}
