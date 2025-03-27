using System;

namespace Parking_Zone.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string GetEmailConfirmationTemplate(string callbackUrl)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Confirm Your Email</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 10px; text-align: center; }}
        .content {{ padding: 20px; border: 1px solid #ddd; }}
        .button {{ display: inline-block; background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }}
        .footer {{ margin-top: 20px; text-align: center; font-size: 12px; color: #777; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Email Confirmation</h1>
        </div>
        <div class=""content"">
            <p>Thank you for registering with Parking Zone. Please confirm your email address by clicking the button below:</p>
            <p style=""text-align: center;"">
                <a href=""{callbackUrl}"" class=""button"">Confirm Email</a>
            </p>
            <p>If you did not create an account, you can ignore this email.</p>
        </div>
        <div class=""footer"">
            <p>&copy; {DateTime.Now.Year} Parking Zone. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        public string GetPasswordResetTemplate(string callbackUrl)
        {
            return GetPasswordResetTemplate(callbackUrl, "User");
        }
        
        public string GetPasswordResetTemplate(string callbackUrl, string userName)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Reset Your Password</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2196F3; color: white; padding: 10px; text-align: center; }}
        .content {{ padding: 20px; border: 1px solid #ddd; }}
        .button {{ display: inline-block; background-color: #2196F3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }}
        .footer {{ margin-top: 20px; text-align: center; font-size: 12px; color: #777; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Password Reset</h1>
        </div>
        <div class=""content"">
            <p>Hello {userName},</p>
            <p>You have requested to reset your password. Please click the button below to set a new password:</p>
            <p style=""text-align: center;"">
                <a href=""{callbackUrl}"" class=""button"">Reset Password</a>
            </p>
            <p>If you did not request a password reset, you can ignore this email.</p>
        </div>
        <div class=""footer"">
            <p>&copy; {DateTime.Now.Year} Parking Zone. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        public string GetAccountLockedTemplate()
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Account Locked</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #F44336; color: white; padding: 10px; text-align: center; }}
        .content {{ padding: 20px; border: 1px solid #ddd; }}
        .footer {{ margin-top: 20px; text-align: center; font-size: 12px; color: #777; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Account Locked</h1>
        </div>
        <div class=""content"">
            <p>Your account has been locked due to multiple failed login attempts.</p>
            <p>For security reasons, we have temporarily locked your account. Please contact our support team to unlock your account or wait for 30 minutes before trying again.</p>
        </div>
        <div class=""footer"">
            <p>&copy; {DateTime.Now.Year} Parking Zone. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        public string GetWelcomeTemplate(string userName)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Welcome to Parking Zone</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #673AB7; color: white; padding: 10px; text-align: center; }}
        .content {{ padding: 20px; border: 1px solid #ddd; }}
        .button {{ display: inline-block; background-color: #673AB7; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }}
        .footer {{ margin-top: 20px; text-align: center; font-size: 12px; color: #777; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Welcome to Parking Zone</h1>
        </div>
        <div class=""content"">
            <p>Hello {userName},</p>
            <p>Thank you for joining Parking Zone! We're excited to have you as a member of our community.</p>
            <p>With your new account, you can:</p>
            <ul>
                <li>Reserve parking spots in advance</li>
                <li>Manage your parking history</li>
                <li>Receive notifications about your reservations</li>
                <li>Access special offers and discounts</li>
            </ul>
            <p style=""text-align: center;"">
                <a href=""https://parkingzone.com/dashboard"" class=""button"">Go to Dashboard</a>
            </p>
        </div>
        <div class=""footer"">
            <p>&copy; {DateTime.Now.Year} Parking Zone. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}