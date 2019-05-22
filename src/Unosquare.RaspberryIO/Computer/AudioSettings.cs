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

        private readonly string[] _errorMess = { "Invalid", "Unable" };

        /// <summary>
        /// Gets the current audio state.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <returns>An <see cref="AudioState"/> object.</returns>
        /// <exception cref="InvalidOperationException">Invalid command, card number or control name.</exception>
        public async Task<AudioState> GetState(int cardNumber = DefaultCardNumber, string controlName = DefaultControlName)
        {
            var volumeInfo = await ProcessRunner.GetProcessOutputAsync("amixer", $"-c {cardNumber} get {controlName}").ConfigureAwait(false);

            var lines = volumeInfo.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (!lines.Any())
                throw new InvalidOperationException("Invalid command.");

            if (_errorMess.Any(x => lines[0].Contains(x)))
                throw new InvalidOperationException(lines[0]);

            var volumeLine = lines
                .FirstOrDefault(x => x.Trim()
                    .StartsWith("Mono:", StringComparison.OrdinalIgnoreCase));

            if (volumeLine == null)
                throw new InvalidOperationException("Unexpected output from 'amixer'.");

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
        /// Sets the volume percentage.
        /// </summary>
        /// <param name="level">The percentage level.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Invalid card number or control name.</exception>
        public Task SetVolumePercentage(int level, int cardNumber = DefaultCardNumber, string controlName = DefaultControlName) =>
            SetAudioCommand($"{level}%", cardNumber, controlName);

        /// <summary>
        /// Sets the volume by decibels.
        /// </summary>
        /// <param name="decibels">The decibels.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Invalid card number or control name.</exception>
        public Task SetVolumeByDecibels(float decibels, int cardNumber = DefaultCardNumber, string controlName = DefaultControlName) =>
            SetAudioCommand($"{decibels}dB", cardNumber, controlName);

        /// <summary>
        /// Increments the volume by decibels.
        /// </summary>
        /// <param name="decibels">The decibels to increment or decrement.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Invalid card number or control name.</exception>
        public Task IncrementVolume(float decibels, int cardNumber = DefaultCardNumber, string controlName = DefaultControlName) =>
            SetAudioCommand($"{decibels}dB{(decibels < 0 ? "-" : "+")}", cardNumber, controlName);

        /// <summary>
        /// Toggles the mute state.
        /// </summary>
        /// <param name="mute">if set to <c>true</c>, mutes the audio.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Invalid card number or control name.</exception>
        public Task ToggleMute(bool mute, int cardNumber = DefaultCardNumber, string controlName = DefaultControlName) =>
            SetAudioCommand(mute ? "mute" : "unmute", cardNumber, controlName);

        private static async Task<string> SetAudioCommand(string command, int cardNumber = DefaultCardNumber, string controlName = DefaultControlName)
        {
            var taskResult = await ProcessRunner.GetProcessOutputAsync("amixer", $"-q -c {cardNumber} -- set {controlName} {command}").ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(taskResult))
                throw new InvalidOperationException(taskResult.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).First());

            return taskResult;
        }
    }
}
