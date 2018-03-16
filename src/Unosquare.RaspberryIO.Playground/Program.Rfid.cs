namespace Unosquare.RaspberryIO.Playground
{
    using Peripherals;
    using Swan;

    public partial class Program
    {
        private static void TestRfidController()
        {
            var device = new RFIDControllerMfrc522();

            while (true)
            {
                // If a card is found
                if (device.DetectCard() == RFIDControllerMfrc522.Status.AllOk)
                    "Card detected".Info();

                // Get the UID of the card
                var uidResponse = device.ReadCardUniqueId();

                // If we have the UID, continue
                if (uidResponse.Status == RFIDControllerMfrc522.Status.AllOk)
                {
                    var cardUid = uidResponse.Data;

                    // Print UID
                    $"Card UID: {cardUid[0]},{cardUid[1]},{cardUid[2]},{cardUid[3]}".Info();

                    // Select the scanned tag
                    device.SelectCardUniqueId(cardUid);

                    // Check if authenticated
                    if (device.AuthenticateCard1A(RFIDControllerMfrc522.DefaultAuthKey, cardUid) == RFIDControllerMfrc522.Status.AllOk)
                    {
                        device.ReadRegister(RFIDControllerMfrc522.Register.Status2Reg);
                        device.ClearCardSelection();
                    }
                    else
                    {
                        "Authentication error".Error();
                    }
                }
            }
        }
    }
}
