using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fcm.Interfaces
{
    public interface IOutputMessage
    {
        void AddOutputMessage( string outputMessage );
        void AddErrorMessage( string errorMessage );
        void UpdateProgressBar( double value, DateTime estimatedTime, int documentsToBeGenerated = 0 );
        void Activate();
    }
}
