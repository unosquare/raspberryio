namespace Unosquare.RaspberryIO.Playground
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Unosquare.RaspberryIO.Peripherals;
    using Unosquare.Swan;

    public partial class Program
    {
        private static readonly Dictionary<ConsoleKey, string> RfidOptions = new Dictionary<ConsoleKey, string>
        {
            { ConsoleKey.D, "Detect Card" },
            { ConsoleKey.R, "Read Card" },
            { ConsoleKey.W, "Write Card" },
            { ConsoleKey.I, "Read Card Sectors" },

        };

        public static async Task ShowRfidMenu()
        {
            var exit = false;
            bool pressKey;

            do
            {
                Console.Clear();
                pressKey = true;

                var mainOption = "Rfid".ReadPrompt(RfidOptions, "Esc to exit this menu");

                switch (mainOption.Key)
                {
                    case ConsoleKey.D:
                        CardDetected();
                        break;
                    case ConsoleKey.W:
                        WriteCard();
                        break;
                    case ConsoleKey.R:
                        ReadCard();
                        break;
                    case ConsoleKey.I:
                        ReadAllCardSectors();
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        pressKey = false;
                        break;
                    default:
                        pressKey = false;
                        break;
                }

                if (pressKey)
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    Console.WriteLine("Press any key to continue . . .");
                    Console.ReadKey(true);
                }
            }
            while (!exit);
        }

        public static void TestRfidController()
        {
            "Testing RFID".Info();
            var device = new RFIDControllerMfrc522(Pi.Spi.Channel0, 500000, Pi.Gpio[18]);

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

                    for (var b = 0; b < 3; b++)
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

        private static void CardDetected()
        {
            Console.Clear();
            "Testing RFID".Info();
            var device = new RFIDControllerMfrc522(Pi.Spi.Channel0, 500000, Pi.Gpio[18]);

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

                break;
            }
        }

        private static void WriteCard()
        {
            Console.Clear();
            "Testing RFID".Info();
            var device = new RFIDControllerMfrc522(Pi.Spi.Channel0, 500000, Pi.Gpio[18]);

            Console.WriteLine("Insert a message to be writed in the card, 16 characters only");
            var userInput = Console.ReadLine().Truncate(16);
            Console.WriteLine("Put a card over the sensor");

            while (true)
            {
                // If a card is found
                if (device.DetectCard() != RFIDControllerMfrc522.Status.AllOk) continue;

                // Get the UID of the card
                var uidResponse = device.ReadCardUniqueId();

                // If we have the UID, continue
                if (uidResponse.Status != RFIDControllerMfrc522.Status.AllOk) continue;

                var cardUid = uidResponse.Data;

                // Select the scanned tag
                device.SelectCardUniqueId(cardUid);

                // Writing data to sector 1 blocks
                // Authenticate sector
                if (device.AuthenticateCard1A(RFIDControllerMfrc522.DefaultAuthKey, cardUid, 11) == RFIDControllerMfrc522.Status.AllOk)
                {
                    userInput = (userInput + new string(' ', 16)).Truncate(16);
                    var data = System.Text.Encoding.ASCII.GetBytes(userInput);
                    device.CardWriteData(8, data);
                }

                device.ClearCardSelection();
                "Writed".Info();
                break;
            }
        }

        private static void ReadCard()
        {
            Console.Clear();
            "Testing RFID".Info();
            var device = new RFIDControllerMfrc522(Pi.Spi.Channel0, 500000, Pi.Gpio[18]);

            while (true)
            {
                // If a card is found
                if (device.DetectCard() != RFIDControllerMfrc522.Status.AllOk) continue;

                // Get the UID of the card
                var uidResponse = device.ReadCardUniqueId();

                // If we have the UID, continue
                if (uidResponse.Status != RFIDControllerMfrc522.Status.AllOk) continue;

                var cardUid = uidResponse.Data;
                // Print UID
                $"Card UID: {cardUid[0]},{cardUid[1]},{cardUid[2]},{cardUid[3]}".Info();

                // Select the scanned tag
                device.SelectCardUniqueId(cardUid);

                // Reading data
                var continueReading = true;

                // Authenticate sector
                if (device.AuthenticateCard1A(RFIDControllerMfrc522.DefaultAuthKey, cardUid, 11) == RFIDControllerMfrc522.Status.AllOk)
                {
                    $"Sector {2}".Info();
                    for (var b = 0; b < 3 && continueReading; b++)
                    {
                        var data = device.CardReadData((byte)((4 * 2) + b));
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

                device.ClearCardSelection();
                break;
            }
        }

        private static void ReadAllCardSectors()
        {
            Console.Clear();
            "Testing RFID".Info();
            var device = new RFIDControllerMfrc522(Pi.Spi.Channel0, 500000, Pi.Gpio[18]);

            while (true)
            {
                // If a card is found
                if (device.DetectCard() != RFIDControllerMfrc522.Status.AllOk) continue;

                // Get the UID of the card
                var uidResponse = device.ReadCardUniqueId();

                // If we have the UID, continue
                if (uidResponse.Status != RFIDControllerMfrc522.Status.AllOk) continue;

                var cardUid = uidResponse.Data;

                // Print UID
                $"Card UID: {cardUid[0]},{cardUid[1]},{cardUid[2]},{cardUid[3]}".Info();

                // Select the scanned tag
                device.SelectCardUniqueId(cardUid);

                // Reading data
                var continueReading = true;
                for (var s = 0; s < 16 && continueReading; s++)
                {
                    // Authenticate sector
                    if (device.AuthenticateCard1A(RFIDControllerMfrc522.DefaultAuthKey, cardUid, 11) == RFIDControllerMfrc522.Status.AllOk)
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

                            $"  Block {b} ({data.Data.Length} bytes): {string.Join(" ", data.Data.Select(x => x.ToString("X2")))}"
                                .Info();
                        }
                    }
                    else
                    {
                        "Authentication error".Error();
                        break;
                    }
                }

                device.ClearCardSelection();
                break;
            }
        }
    }
}
