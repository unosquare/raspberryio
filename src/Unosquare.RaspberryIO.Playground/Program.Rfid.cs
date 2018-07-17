namespace Unosquare.RaspberryIO.Playground
{
    using Peripherals;
    using Swan;
    using System.Linq;

    public partial class Program
    {
        private static void TestRfidController()
        {
            "Testing RFID".Info();
            var device = new RFIDControllerMfrc522(Pi.Spi.Channel1, 500000, Pi.Gpio[18]);

            while (true)
            {
                // If a card is found
                if (device.DetectCard() != RFIDControllerMfrc522.Status.AllOk) continue;
                "Card detected".Info();

                // Get the UID of the card
                var uidResponse = device.ReadCardUniqueId();

                // If we have the UID, continue
                if (uidResponse.Status != RFIDControllerMfrc522.Status.AllOk) continue;

                var cardUid = uidResponse.Data;

                // Print UID
                $"Card UID: {cardUid[0]},{cardUid[1]},{cardUid[2]},{cardUid[3]}".Info();

                // Select the scanned tag
                device.SelectCardUniqueId(cardUid);

                // Writing data to sector 1 blocks
                // Authenticate sector
                if (device.AuthenticateCard1A(RFIDControllerMfrc522.DefaultAuthKey, cardUid, 7) == RFIDControllerMfrc522.Status.AllOk)
                {
                    var data = new byte[16 * 3];
                    for (var x = 0; x < data.Length; x++)
                    {
                        data[x] = (byte)(x + 65);
                    }

                    for(var b = 0; b < 3; b++)
                    {
                        device.CardWriteData((byte)(4 + b), data.Skip(b * 16).Take(16).ToArray());
                    }
                }

                // Reading data
                var continueReading = true;
                for (var s = 0; s < 16 && continueReading; s++)
                {
                    // Authenticate sector
                    if (device.AuthenticateCard1A(RFIDControllerMfrc522.DefaultAuthKey, cardUid, (byte)((4 * s) + 3)) == RFIDControllerMfrc522.Status.AllOk)
                    {
                        $"Sector {s}".Info();
                        for (var b = 0; b < 3 && continueReading; b++)
                        {
                            var data = device.CardReadData((byte)((4 * s) + b));
                            if (data.Status != RFIDControllerMfrc522.Status.AllOk)
                            {
                                continueReading = false;
                                break;
                            }

                            $"  Block {b} ({data.Data.Length} bytes): {string.Join(" ", data.Data.Select(x => x.ToString("X2")))}".Info();
                        }
                    }
                    else
                    {
                        "Authentication error".Error();
                        break;
                    }
                }

                device.ClearCardSelection();
            }
        }
    }
}
