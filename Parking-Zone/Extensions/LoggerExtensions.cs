using System;
using Microsoft.Extensions.Logging;

namespace Parking_Zone.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogUserAction(this ILogger logger, string userId, string action, string details = null)
        {
            logger.LogInformation(
                "User {UserId} performed action {Action}. Details: {Details}",
                userId,
                action,
                details ?? "No additional details"
            );
        }

        public static void LogVehicleEntry(this ILogger logger, string licensePlate, string gateId, string operatorId)
        {
            logger.LogInformation(
                "Vehicle {LicensePlate} entered through gate {GateId} by operator {OperatorId}",
                licensePlate,
                gateId,
                operatorId
            );
        }

        public static void LogVehicleExit(this ILogger logger, string licensePlate, string gateId, string operatorId, decimal fee)
        {
            logger.LogInformation(
                "Vehicle {LicensePlate} exited through gate {GateId} by operator {OperatorId}. Fee: {Fee}",
                licensePlate,
                gateId,
                operatorId,
                fee
            );
        }

        public static void LogPayment(this ILogger logger, string transactionId, decimal amount, string paymentMethod)
        {
            logger.LogInformation(
                "Payment received for transaction {TransactionId}. Amount: {Amount}, Method: {PaymentMethod}",
                transactionId,
                amount,
                paymentMethod
            );
        }

        public static void LogGateOperation(this ILogger logger, string gateId, string operation, string status)
        {
            logger.LogInformation(
                "Gate {GateId} {Operation} operation completed with status: {Status}",
                gateId,
                operation,
                status
            );
        }

        public static void LogHardwareStatus(this ILogger logger, string deviceId, string deviceType, string status)
        {
            logger.LogInformation(
                "{DeviceType} {DeviceId} status changed to: {Status}",
                deviceType,
                deviceId,
                status
            );
        }

        public static void LogSystemError(this ILogger logger, Exception ex, string context)
        {
            logger.LogError(
                ex,
                "System error occurred in {Context}. Message: {Message}",
                context,
                ex.Message
            );
        }

        public static void LogSecurityEvent(this ILogger logger, string userId, string eventType, string details)
        {
            logger.LogWarning(
                "Security event detected. User: {UserId}, Event: {EventType}, Details: {Details}",
                userId,
                eventType,
                details
            );
        }

        public static void LogDatabaseOperation(this ILogger logger, string operation, string entity, bool success, string details = null)
        {
            if (success)
            {
                logger.LogInformation(
                    "Database {Operation} on {Entity} completed successfully. Details: {Details}",
                    operation,
                    entity,
                    details ?? "No additional details"
                );
            }
            else
            {
                logger.LogError(
                    "Database {Operation} on {Entity} failed. Details: {Details}",
                    operation,
                    entity,
                    details ?? "No additional details"
                );
            }
        }

        public static void LogPerformanceMetric(this ILogger logger, string operation, long elapsedMilliseconds)
        {
            logger.LogInformation(
                "Performance metric - Operation: {Operation}, Duration: {Duration}ms",
                operation,
                elapsedMilliseconds
            );
        }
    }
} 