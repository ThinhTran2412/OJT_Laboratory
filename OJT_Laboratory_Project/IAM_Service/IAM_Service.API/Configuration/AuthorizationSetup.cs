//namespace IAM_Service.API.Configuration
//{
//    public static class AuthorizationSetup
//    {
//        /// <summary>
//        /// The privilege claim type
//        /// </summary>
//        private const string PrivilegeClaimType = "privilege";

//        /// <summary>
//        /// Adds the privilege policies.
//        /// </summary>
//        /// <param name="services">The services.</param>
//        /// <returns></returns>
//        public static IServiceCollection AddPrivilegePolicies(this IServiceCollection services)
//        {
//            services.AddAuthorization(options =>
//            {
//                // Patient Test Order Policies
//                options.AddPolicy("CanViewTestOrders", p => p.RequireClaim(PrivilegeClaimType, "READ_ONLY"));
//                options.AddPolicy("CanCreateTestOrder", p => p.RequireClaim(PrivilegeClaimType, "CREATE_TEST_ORDER"));
//                options.AddPolicy("CanModifyTestOrder", p => p.RequireClaim(PrivilegeClaimType, "MODIFY_TEST_ORDER"));
//                options.AddPolicy("CanDeleteTestOrder", p => p.RequireClaim(PrivilegeClaimType, "DELETE_TEST_ORDER"));
//                options.AddPolicy("CanReviewTestOrder", p => p.RequireClaim(PrivilegeClaimType, "REVIEW_TEST_ORDER"));
//                options.AddPolicy("CanAddComment", p => p.RequireClaim(PrivilegeClaimType, "ADD_COMMENT"));
//                options.AddPolicy("CanModifyComment", p => p.RequireClaim(PrivilegeClaimType, "MODIFY_COMMENT"));
//                options.AddPolicy("CanDeleteComment", p => p.RequireClaim(PrivilegeClaimType, "DELETE_COMMENT"));

//                // Configuration Policies
//                options.AddPolicy("CanViewConfiguration", p => p.RequireClaim(PrivilegeClaimType, "VIEW_CONFIGURATION"));
//                options.AddPolicy("CanCreateConfiguration", p => p.RequireClaim(PrivilegeClaimType, "CREATE_CONFIGURATION"));
//                options.AddPolicy("CanModifyConfiguration", p => p.RequireClaim(PrivilegeClaimType, "MODIFY_CONFIGURATION"));
//                options.AddPolicy("CanDeleteConfiguration", p => p.RequireClaim(PrivilegeClaimType, "DELETE_CONFIGURATION"));

//                // User Management Policies
//                options.AddPolicy("CanViewUser", p => p.RequireClaim(PrivilegeClaimType, "VIEW_USER"));
//                options.AddPolicy("CanCreateUser", p => p.RequireClaim(PrivilegeClaimType, "CREATE_USER"));
//                options.AddPolicy("CanModifyUser", p => p.RequireClaim(PrivilegeClaimType, "MODIFY_USER"));
//                options.AddPolicy("CanDeleteUser", p => p.RequireClaim(PrivilegeClaimType, "DELETE_USER"));
//                options.AddPolicy("CanLockUnlockUser", p => p.RequireClaim(PrivilegeClaimType, "LOCK_UNLOCK_USER"));

//                // Role Management Policies
//                options.AddPolicy("CanViewRole", p => p.RequireClaim(PrivilegeClaimType, "VIEW_ROLE"));
//                options.AddPolicy("CanCreateRole", p => p.RequireClaim(PrivilegeClaimType, "CREATE_ROLE"));
//                options.AddPolicy("CanUpdateRole", p => p.RequireClaim(PrivilegeClaimType, "UPDATE_ROLE"));
//                options.AddPolicy("CanDeleteRole", p => p.RequireClaim(PrivilegeClaimType, "DELETE_ROLE"));

//                // Lab Management & Instrument Policies
//                options.AddPolicy("CanViewEventLogs", p => p.RequireClaim(PrivilegeClaimType, "VIEW_EVENT_LOGS"));
//                options.AddPolicy("CanAddReagents", p => p.RequireClaim(PrivilegeClaimType, "ADD_REAGENTS"));
//                options.AddPolicy("CanModifyReagents", p => p.RequireClaim(PrivilegeClaimType, "MODIFY_REAGENTS"));
//                options.AddPolicy("CanDeleteReagents", p => p.RequireClaim(PrivilegeClaimType, "DELETE_REAGENTS"));
//                options.AddPolicy("CanAddInstrument", p => p.RequireClaim(PrivilegeClaimType, "ADD_INSTRUMENT"));
//                options.AddPolicy("CanViewInstrument", p => p.RequireClaim(PrivilegeClaimType, "VIEW_INSTRUMENT"));
//                options.AddPolicy("CanActivateDeactivateInstrument", p => p.RequireClaim(PrivilegeClaimType, "ACTIVATE_DEACTIVATE_INSTRUMENT"));
//                options.AddPolicy("CanExecuteBloodTesting", p => p.RequireClaim(PrivilegeClaimType, "EXECUTE_BLOOD_TESTING"));

//                // Policy gộp cho Admin (ví dụ: yêu cầu tất cả quyền Config)
//                options.AddPolicy("CanManageConfiguration", p => p.RequireClaim(PrivilegeClaimType, new[] {
//                    "VIEW_CONFIGURATION", "CREATE_CONFIGURATION", "MODIFY_CONFIGURATION", "DELETE_CONFIGURATION"
//                }));
//            });

//            return services;
//        }
//    }
//}