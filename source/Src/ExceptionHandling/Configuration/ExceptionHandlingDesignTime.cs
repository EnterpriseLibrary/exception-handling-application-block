// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseLibrary.ExceptionHandling.Configuration
{
    
    internal static class ExceptionHandlingDesignTime
    {
        public static class ViewModelTypeNames
        {
            public const string ExceptionHandlingSectionViewModel =
                "EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ExceptionHandlingSectionViewModel, EnterpriseLibrary.Configuration.DesignTime";

            public const string ExceptionPolicyDataViewModel =
                "EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ExceptionPolicyDataViewModel, EnterpriseLibrary.Configuration.DesignTime";

            public const string ExceptionTypeDataViewModel =
                "EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ExceptionTypeDataViewModel, EnterpriseLibrary.Configuration.DesignTime";

            public const string ExceptionHandlerDataViewModel =
                "EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ExceptionHandlerDataViewModel, EnterpriseLibrary.Configuration.DesignTime";
        }

        public static class CommandTypeNames
        {
            public const string AddExceptionPolicyCommand = "EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.AddExceptionPolicyCommand, EnterpriseLibrary.Configuration.DesignTime";

            public const string AddExceptionHandlingBlockCommand = "EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.AddExceptionHandlingBlockCommand, EnterpriseLibrary.Configuration.DesignTime";

            public const string AddExceptionTypeCommand = "EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ExceptionTypeAddCommand, EnterpriseLibrary.Configuration.DesignTime";
        }

        public static class ValidatorTypes
        {
            public const string NameValueCollectionValidator = "EnterpriseLibrary.Configuration.Design.Validation.NameValueCollectionValidator, EnterpriseLibrary.Configuration.DesignTime";
        }
    }
}
