using System;

namespace MackkadoITFramework.Interfaces
{
    public interface IOutputMessage
    {
        void AddOutputMessage( string outputMessage, string processName, string userID );
        void AddErrorMessage( string errorMessage, string processName, string userID );
        void UpdateProgressBar( double value, DateTime estimatedTime, int documentsToBeGenerated = 0 );
        void Activate();
    }
}
