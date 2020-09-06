using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace MainModuleSecurity
{
    public class TokenManager
    {
        public static string EncodeToken(JwtPayload jwtPayload, string secret)
        {
            const string algorithm = "HMAC256";

            var header = new ProcoorMainModuleDTO.SecurityModel.JwtHeader
            {
                typ = "JWT",
                alg = algorithm
            };

            var jwt = Base64Encode(JsonConvert.SerializeObject(header)) + "." + Base64Encode(JsonConvert.SerializeObject(jwtPayload));

            jwt += "." + Sign(jwt, secret);

            return jwt;
        }

        public static ProcoorMainModuleDTO.SecurityModel.JwtPayload DecodeToken(string token, string secret, out bool validated)
        {
            var jwtPayload = new ProcoorMainModuleDTO.SecurityModel.JwtPayload();

            var segments = token.Split('.');

            var invalidToken = segments.Length != 3;

            var PayLoadBefore = segments[1];
            if (!invalidToken)
            {
                try
                {
                    PayLoadBefore = PayLoadBefore.Replace(" ", "+");

                    int mod4 = PayLoadBefore.Length % 4;
                    if (mod4 > 0)
                    {
                        PayLoadBefore += new string('=', 4 - mod4);
                    }
                    jwtPayload = JsonConvert.DeserializeObject(
                        Encoding.UTF8.GetString(Base64Decode(PayLoadBefore)), typeof(ProcoorMainModuleDTO.SecurityModel.JwtPayload));

                    var rawSignature = segments[0] + '.' + segments[1];

                    validated = Verify(rawSignature, secret, segments[2]);
                }
                catch (Exception exception)
                {
                    validated = false;
                    string str = exception.Message;
                }
            }
            else
            {
                validated = false;
            }

            validated = true;

            return jwtPayload;
        }

        private static bool Verify(string rawSignature, string secret, string signature)
        {
            return signature == Sign(rawSignature, secret);
        }

        private static string Sign(string str, string key)
        {
            var encoding = new ASCIIEncoding();

            byte[] signature;

            using (var crypto = new HMACSHA256(encoding.GetBytes(key)))
            {
                signature = crypto.ComputeHash(encoding.GetBytes(str));
            }

            return Base64Encode(signature);
        }

        public static string Base64Encode(dynamic obj)
        {
            Type strType = obj.GetType();

            var base64EncodedValue = Convert.ToBase64String(strType.Name.ToLower() == "string" ? Encoding.UTF8.GetBytes(obj) : obj);

            return base64EncodedValue;
        }

        public static dynamic Base64Decode(string str)
        {
            var base64DecodedValue = Convert.FromBase64String(str);

            return base64DecodedValue;
        }
        public static userInfo GetUserInfo(IIdentity token)
        {
            var identity = (ClaimsIdentity)token;

            List<Claim> claims = identity.Claims.ToList();
            var usrAccount = new userInfo();

            var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (sub != null)
            {
                //Procoor_V4_DatabaseContext.CliamsContext.ClaimsContext
                using (var context = new MainModuleContext())
                {
                    usrAccount = context.accounts.Where(x => x.deletedBy == null && x.userName == sub.Value)
                      .Select(x => new userInfo
                      {
                          id = x.id,
                          contactId = (int)x.contactId,
                          companyId = (int)x.contact.companyId,
                          groupId = (int)(x.groupId ?? -1),
                          isHrManager = (bool)(x.isHrManager ?? false),
                          accountOwnerId = (int)x.accountOwnerId,
                          userType = x.userType,
                          usePermissionsOnLogs = (bool)(x.usePermissionsOnLogs ?? false),
                          //   employeeId = x.contact.hr_employee.Where(e => e.deletedBy == null).Select(e => e.id).FirstOrDefault(),
                      }).FirstOrDefault();

                    if (usrAccount == null)
                    {
                        return new userInfo();
                    }
                    return usrAccount;
                }
            }

            return usrAccount;
        }
        public static userInfo GetUserInfoCore(string identity)
        {
            // var identity = (ClaimsIdentity)token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(identity);
            var tokenS = handler.ReadToken(identity) as JwtSecurityToken;


            List<Claim> claims = tokenS.Claims.ToList();
            var usrAccount = new userInfo();

            var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (sub != null)
            {
                //MainModuleContext      Procoor_V4_DatabaseContext.CliamsContext.ClaimsContext
                using (var context = new MainModuleContext())
                {
                    usrAccount = context.accounts.Where(x => x.deletedBy == null && x.userName == sub.Value)
                      .Select(x => new userInfo
                      {
                          id = x.id,
                          contactId = (int)x.contactId,
                          companyId = (int)x.contact.companyId,
                          groupId = (int)(x.groupId ?? -1),
                          isHrManager = (bool)(x.isHrManager ?? false),
                          accountOwnerId = (int)x.accountOwnerId,
                          userType = x.userType,
                          usePermissionsOnLogs = (bool)(x.usePermissionsOnLogs ?? false),
                          //employeeId = x.contact.hr_employee.Where(e => e.deletedBy == null).Select(e => e.id).FirstOrDefault(),
                      }).FirstOrDefault();

                    if (usrAccount == null)
                    {
                        return new userInfo();
                    }
                    return usrAccount;
                }
            }

            return usrAccount;
        }
        public static int GetUserIdentity(IIdentity token)
        {
            var identity = (ClaimsIdentity)token;

            List<Claim> claims = identity.Claims.ToList();

            var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (sub != null)
            {
                using (var context = new MainModuleContext())
                {
                    try
                    {
                        var usrAccount = context.accounts.SingleOrDefault(x => x.userName == sub.Value);

                        var userId = -1;

                        if (usrAccount != null)
                        {
                            userId = usrAccount.id;
                        }

                        return userId;
                    }
                    catch
                    {
                        throw new Exception();
                    }

                }
            }

            return -1;
        }
        public static int GetUserCompanyId(IIdentity token)
        {
            var identity = (ClaimsIdentity)token;

            List<Claim> claims = identity.Claims.ToList();

            var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (sub != null)
            {
                using (var context = new MainModuleContext())
                {
                    var usrAccount = context.accounts.SingleOrDefault(x => x.userName == sub.Value);

                    var userCompanyId = -1;

                    if (usrAccount != null)
                    {
                        var userContact = context.project_companies_contacts.SingleOrDefault(x => x.accountId == usrAccount.id);

                        if (userContact != null)
                        {
                            userCompanyId = userContact.companyId ?? -1;
                        }
                    }

                    return userCompanyId;
                }
            }

            return -1;
        }
        public static int GetOwnerIdentity(IIdentity token)
        {
            var identity = (ClaimsIdentity)token;

            List<Claim> claims = identity.Claims.ToList();

            var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (sub != null)
            {
                using (var context = new MainModuleContext())
                {
                    var usrAccount = context.accounts.SingleOrDefault(x => x.userName == sub.Value);

                    int? accountOwnerId = -1;

                    if (usrAccount != null)
                    {
                        accountOwnerId = usrAccount.accountOwnerId;
                    }

                    return (int)accountOwnerId;
                }
            }

            return -1;
        }
        public static string GetUserType(IIdentity token)
        {
            var identity = (ClaimsIdentity)token;

            List<Claim> claims = identity.Claims.ToList();

            var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (sub != null)
            {
                using (var context = new MainModuleContext())
                {
                    var usrAccount = context.accounts.SingleOrDefault(x => x.userName == sub.Value);

                    var userType = "";

                    if (usrAccount != null)
                    {
                        userType = usrAccount.userType;
                    }

                    return userType;
                }
            }

            return "";
        }
        public static int GetGroupId(IIdentity token)
        {
            var identity = (ClaimsIdentity)token;

            List<Claim> claims = identity.Claims.ToList();

            var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (sub != null)
            {
                using (var context = new MainModuleContext())
                {
                    var usrAccount = context.accounts.SingleOrDefault(x => x.userName == sub.Value);

                    var groupId = -1;

                    if (usrAccount != null)
                    {
                        groupId = usrAccount.groupId ?? -1;
                    }

                    return groupId;
                }
            }

            return -1;
        }
        //public static bool GetPasswordEdit(IIdentity token)
        //{
        //    var identity = (ClaimsIdentity)token;

        //    List<Claim> claims = identity.Claims.ToList();

        //    var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

        //    if (sub != null)
        //    {
        //        using (var context =new ProcoorContext())
        //        {
        //            var usrAccount = context.accounts.SingleOrDefault(x => x.userName == sub.Value);

        //            var passwordEdit = false;

        //            if (usrAccount != null)
        //            {
        //                passwordEdit = usrAccount.passwordEdit ?? false;
        //            }

        //            return passwordEdit;
        //        }
        //    }

        //    return false;
        //}

        public static bool GetIsHrManager(IIdentity token)
        {
            var identity = (ClaimsIdentity)token;

            List<Claim> claims = identity.Claims.ToList();

            var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (sub != null)
            {
                using (var context = new MainModuleContext())
                {
                    var usrAccount = context.accounts.SingleOrDefault(x => x.userName == sub.Value);

                    var isHrManager = false;

                    if (usrAccount != null)
                    {
                        isHrManager = usrAccount.isHrManager ?? false;
                    }

                    return isHrManager;
                }
            }

            return false;
        }
        public static bool GetUseLogsPeremissions(IIdentity token)
        {
            var identity = (ClaimsIdentity)token;

            List<Claim> claims = identity.Claims.ToList();

            var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (sub != null)
            {
                using (var context = new MainModuleContext())
                {
                    var usrAccount = context.accounts.SingleOrDefault(x => x.userName == sub.Value);

                    var useLogsPermissions = false;

                    if (usrAccount != null)
                    {
                        useLogsPermissions = usrAccount.usePermissionsOnLogs ?? false;
                    }

                    return useLogsPermissions;
                }
            }

            return false;
        }

        public static int GetEmployeeId(IIdentity token)
        {
            var employeeId = -1;

            //var identity = (ClaimsIdentity)token;

            //List<Claim> claims = identity.Claims.ToList();

            //var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            //if (sub != null)
            //{
            //    using (var context =new ProcoorContext())
            //    {
            //        var usrAccount = context.accounts.SingleOrDefault(x => x.userName == sub.Value);

            //        var employeeId = -1;

            //        if (usrAccount != null)
            //        {
            //            var userContact = context.project_companies_contacts.SingleOrDefault(x => x.accountId == usrAccount.id);

            //            if (userContact != null)
            //            {
            //                var employee = context.hr_employee.SingleOrDefault(x => x.contactId == userContact.id);

            //                if (employee != null)
            //                {
            //                    employeeId = employee.id;
            //                }

            //            }
            //        }

            //        return employeeId;
            //    }
            //}


            return employeeId;
        }
        public static int GetContactId(System.Security.Principal.IIdentity token)
        {
            var identity = (ClaimsIdentity)token;

            List<Claim> claims = identity.Claims.ToList();

            var sub = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (sub != null)
            {
                using (var context = new MainModuleContext())
                {
                    var usrAccount = context.accounts.SingleOrDefault(x => x.userName == sub.Value);

                    var contactId = -1;

                    if (usrAccount != null)
                    {
                        var userContact = context.project_companies_contacts.SingleOrDefault(x => x.accountId == usrAccount.id);

                        if (userContact != null)
                        {
                            contactId = userContact.id;
                        }
                    }

                    return contactId;
                }
            }

            return -1;
        }
    }
    public class userInfo
    {
        public int id { get; set; }
        public int contactId { get; set; }
        public int companyId { get; set; }
        public int employeeId { get; set; }
        public int groupId { get; set; }
        public int accountOwnerId { get; set; }
        public string userType { get; set; }
        public bool usePermissionsOnLogs { get; set; }
        public bool isHrManager { get; set; }
    }
}
