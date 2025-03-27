using Parking_Zone.Models;
using Parking_Zone.Enums;
using System;

namespace Parking_Zone.Extensions
{
    public static class ReservationExtensions
    {
        public static string GetVehicleNumber(this Reservation reservation)
        {
            return reservation.Vehicle?.LicensePlate ?? string.Empty;
        }

        public static Guid? GetParkingSpotId(this Reservation reservation)
        {
            return reservation.ParkingSpot?.Id;
        }

        public static DateTime? GetEndTime(this Reservation reservation)
        {
            return reservation.EndTime;
        }

        public static bool IsStatus(this Reservation reservation, string statusString)
        {
            return Enum.TryParse<Parking_Zone.Models.ReservationStatus>(statusString, out var status) &&
                   reservation.Status == status;
        }

        public static DateTime? GetConfirmedAt(this Reservation reservation)
        {
            return reservation.LastUpdated;
        }

        public static DateTime? GetCompletedAt(this Reservation reservation)
        {
            return reservation.CompletedDateTime;
        }

        public static string? GetAppUserId(this Reservation reservation)
        {
            return reservation.AppUserId;
        }

        public static Guid? GetParkingSpot(this Reservation reservation)
        {
            return reservation.ParkingSpotId;
        }

        public static Guid? ToNullableGuid(this int? value)
        {
            return value.HasValue ? new Guid(value.Value.ToString()) : null;
        }
    }
}
