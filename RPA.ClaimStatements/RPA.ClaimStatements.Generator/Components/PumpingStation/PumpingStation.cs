using System;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation
{
    public class PumpingStation : IPumpingStation
    {
        ISITIImportService sitiImportService;
        IXBImportService xbImportService;
        IAPImportService apImportService;
        IARImportService arImportService;

        public PumpingStation(ISITIImportService sitiImportService, IXBImportService xbImportService, IAPImportService apImportService, IARImportService arImportService)
        {
            this.sitiImportService = sitiImportService;
            this.xbImportService = xbImportService;
            this.apImportService = apImportService;
            this.arImportService = arImportService;
        }

        public void Activate()
        {
            Console.ForegroundColor = ConsoleColor.Green;            
            Console.WriteLine("Pumping Station activated");
            Console.ResetColor();

            sitiImportService.Import();
            xbImportService.Import();
            apImportService.Import();
            arImportService.Import();
        }
    }
}
