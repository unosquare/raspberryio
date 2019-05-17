namespace Unosquare.RaspberryIO.Computer
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Swan.Abstractions;
    using Swan.Components;

    /// <summary>
    /// Settings for audio device.
    /// </summary>
    public class AudioSettings : SingletonBase<AudioSettings>
    {
        /// <summary>
        /// Get the resultant info of the current volume state.
        /// </summary>
        /// <param name="deviceName"> Name of the device. </param>
        /// <param name="cardNumber"> card number to get state from. </param>
        /// <returns> Volume state object with current volume control settings info. </returns>
        public static async Task<AudioState> GetAudioDeviceState(string deviceName, int cardNumber)
        {
            var volumeInfo = await ProcessRunner.GetProcessOutputAsync("amixer", $"-c {cardNumber} get {deviceName}").ConfigureAwait(false);
            var volumeLine = volumeInfo.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => x.Trim().StartsWith("Mono:", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

            var sections = volumeLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var level = int.Parse(sections[3].Substring(1, sections[3].Length - 3), System.Globalization.NumberFormatInfo.InvariantInfo);
            var decibels = sections[4].Trim(new[] { '[', ']', 'd', 'B' });
            var decibelsNumber = float.Parse(decibels, System.Globalization.NumberFormatInfo.InvariantInfo);
            var isMute = sections[5].Equals("[off]", StringComparison.CurrentCultureIgnoreCase);

            return new AudioState(level) { CardNumber = cardNumber, DeviceName = deviceName, Decibels = decibelsNumber, IsMute = isMute };
        }

        /// <summary>
        /// Returns the device information.
        /// </summary>
        /// <param name="cardNumber"> Audio card to get state from. </param>
        /// <param name="controlName"> Audio controller name. </param>
        public static async Task GetDeviceInfo(int cardNumber, string controlName)
        {
            var currentState = await GetAudioDeviceState(controlName, cardNumber).ConfigureAwait(false);
            Console.WriteLine(currentState.ToString());
        }

        /// <summary>
        /// Presents a command list to the user .
        /// </summary>
        public static void GetCommandList()
        {
            Console.WriteLine("Audio command list: \n\n");
            Console.WriteLine("vol: Set volume level of an audio device.\n");
            Console.WriteLine("vol+: Increase volume level of an audio device.\n");
            Console.WriteLine("vol-: Decrease volume level of an audio device.\n");
            Console.WriteLine("mute - Mute an audio device.\n");
            Console.WriteLine("info - Get audio settings of an audio device.\n");
            Console.WriteLine("help - Get the audio commands list.\n");
            Console.WriteLine("close - Close the audio settings playground.\n\n");
        }

        /// <summary>
        /// Sets new volume state.
        /// </summary>
        /// <param name="level"> Object containing new state data. </param>
        /// <param name="cardNumber"> Audio card number. </param>
        /// <param name="controlName"> Control name </param>
        /// <returns> Linux command line with audio settings. </returns>
        public async Task SetVolumePercentage(int level, int cardNumber, string controlName)
        {
            var currentState = await GetAudioDeviceState(controlName, cardNumber).ConfigureAwait(false);
            if (currentState.Level != level)
                await SetAudioCommand(cardNumber, $"{level}%", controlName).ConfigureAwait(false);
        }

        /// <summary>
        /// Mutes or unmutes the current card.
        /// </summary>
        /// <param name="mute"> Object containing new state data. </param>
        /// <param name="cardNumber"> Audio card number. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> Linux command line with audio settings. </returns>
        public async Task ToggleMute(bool mute, int cardNumber, string controlName)
        {
            var currentState = await GetAudioDeviceState(controlName, cardNumber).ConfigureAwait(false);
            if (currentState.IsMute != mute)
                await SetAudioCommand(cardNumber, mute ? "mute" : "unmute", controlName).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads a command to write to amixer file for volume control.
        /// </summary>
        /// <param name="cardNumber"> Audio card to modify parameters. </param>
        /// <param name="command"> PCM command for amixer device. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> Performs an async write to amixer. </returns>
        private Task<string> SetAudioCommand(int cardNumber, string command, string controlName) =>
            ProcessRunner.GetProcessOutputAsync("amixer", $"-q -c {cardNumber} -- set {controlName} {command}");

    }
}
