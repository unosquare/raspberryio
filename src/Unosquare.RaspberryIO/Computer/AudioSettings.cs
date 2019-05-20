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
        private const string DefaultControlName = "PCM";
        private const int DefaultCardNumber = 0;

        /// <summary>
        /// Get the resultant info of the current volume state.
        /// </summary>
        /// <param name="cardNumber"> card number to get state from. </param>
        /// <param name="controlName"> controller name. </param>
        /// <returns> Volume state object with current volume control settings info. </returns>
        public static async Task<AudioState> GetAudioDeviceState(int cardNumber = DefaultCardNumber, string controlName = DefaultControlName)
        {
            var volumeInfo = await ProcessRunner.GetProcessOutputAsync("amixer", $"-c {cardNumber} get {controlName}").ConfigureAwait(false);
            var volumeLine = volumeInfo.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Trim().StartsWith("Mono:", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            var sections = volumeLine.Split(new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            var level = int.Parse(sections[3].Substring(1, sections[3].Length - 3),
                                    System.Globalization.NumberFormatInfo.InvariantInfo);

            var decibels = float.Parse(sections[4].Substring(1, sections[4].Length - 4),
                                    System.Globalization.NumberFormatInfo.InvariantInfo);

            var isMute = sections[5].Equals("[off]",
                StringComparison.CurrentCultureIgnoreCase);
            
            return new AudioState(cardNumber, controlName, level, decibels, isMute);
        }

        /// <summary>
        /// Sets new volume state.
        /// </summary>
        /// <param name="level"> Object containing new state data. </param>
        /// <param name="cardNumber"> Audio card number. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> Linux command line with audio settings. </returns>
        public Task SetVolumePercentage(int level, int cardNumber = DefaultCardNumber, string controlName = DefaultControlName) =>
            SetAudioCommand($"{level}%", cardNumber, controlName);

        /// <summary>
        /// Increments or decrements device volume by decibels.
        /// </summary>
        /// <param name="decibels"> How many decibels to increment or decrement. </param>
        /// <param name="cardNumber"> Sound card number. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> A task incrementing or decrementing volume</returns>
        public Task SetVolumeByDecibels(float decibels, int cardNumber = DefaultCardNumber, string controlName = DefaultControlName) =>
            SetAudioCommand($"{decibels}dB", cardNumber, controlName);

        /// <summary>
        /// Increments the volume decibels.
        /// </summary>
        /// <param name="decibels">The decibels.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <returns> Performs a volume increment or decrement. </returns>
        public Task IncrementVolume(float decibels, int cardNumber = DefaultCardNumber, string controlName = DefaultControlName) =>
            SetAudioCommand($"{decibels}dB{(decibels < 0 ? "-" : "+")}", cardNumber, controlName);

        /// <summary>
        /// Mutes or unmutes the current card.
        /// </summary>
        /// <param name="mute"> Object containing new state data. </param>
        /// <param name="cardNumber"> Audio card number. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> Linux command line with audio settings. </returns>
        public Task ToggleMute(bool mute, int cardNumber = DefaultCardNumber, string controlName = DefaultControlName) =>
            SetAudioCommand(mute ? "mute" : "unmute", cardNumber, controlName);

        /// <summary>
        /// Reads a command to write to amixer file for volume control.
        /// </summary>
        /// <param name="command"> PCM command for amixer device. </param>
        /// <param name="cardNumber"> Audio card to modify parameters. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> Performs an async write to amixer. </returns>
        private async Task<string> SetAudioCommand(string command, int cardNumber = DefaultCardNumber, string controlName = DefaultControlName)
        {
            var taskResult = await ProcessRunner.GetProcessOutputAsync("amixer", $"-q -c {cardNumber} -- set {controlName} {command}").ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(taskResult))
                throw new InvalidOperationException(taskResult.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).First());

            return taskResult;
        }
    }
}
