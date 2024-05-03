/*
 * Swagger Basket
 *
 * Корзина товаров
 *
 * The version of the OpenAPI document: 1.0.0
 * 
 * Generated by: https://openapi-generator.tech
 */

using System.ComponentModel;
using System.Runtime.Serialization;
using BasketApp.Api.Adapters.Http.Contract.src.Api.Converters;
using Newtonsoft.Json;

namespace BasketApp.Api.Adapters.Http.Contract.src.Api.Models
{ 
        /// <summary>
        /// Gets or Sets DeliveryPeriod
        /// </summary>
        [TypeConverter(typeof(CustomEnumConverter<DeliveryPeriod>))]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum DeliveryPeriod
        {
            
            /// <summary>
            /// Enum MorningEnum for morning
            /// </summary>
            [EnumMember(Value = "morning")]
            MorningEnum = 1,
            
            /// <summary>
            /// Enum MiddayEnum for midday
            /// </summary>
            [EnumMember(Value = "midday")]
            MiddayEnum = 2,
            
            /// <summary>
            /// Enum EveningEnum for evening
            /// </summary>
            [EnumMember(Value = "evening")]
            EveningEnum = 3,
            
            /// <summary>
            /// Enum NightEnum for night
            /// </summary>
            [EnumMember(Value = "night")]
            NightEnum = 4
        }
}