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
        /// <returns> Volume state object with current volume control settings info. </returns>
        public static async Task<AudioState> GetVolumeState()
        {
            var volumeInfo = await ProcessRunner.GetProcessOutputAsync("amixer", "get PCM").ConfigureAwait(false);
            var volumeLine = volumeInfo.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => x.Trim().StartsWith("Mono:", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

            var sections = volumeLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var level = int.Parse(sections[3].Substring(1, sections[3].Length - 3), System.Globalization.NumberFormatInfo.InvariantInfo);
            var isMute = sections[5].Equals("[off]", StringComparison.CurrentCultureIgnoreCase);
        
            return new AudioState(level) { IsMute = isMute };
        }

        /// <summary>
        /// Sets new volume state.
        /// </summary>
        /// <param name="level"> Object containing new state data. </param>
        /// <param name="cardNumber"> Audio card number. </param>
        /// <param name="controlName"> Control name </param>
        /// <returns> Linux command line with audio settings. </returns>
        public async Task SetVolumePercentage(int level, int cardNumber, string controlName = "PCM")
        {
            var currentState = await GetVolumeState().ConfigureAwait(false);

            if (currentState.Level != level)
                await SetVolumeCommand(cardNumber, $"{level}%", controlName).ConfigureAwait(false);
            else
                return;
        }

        /// <summary>
        /// Sets the volume on decibels.
        /// </summary>
        /// <param name="decibels"> Object containing new state data. </param>
        /// <param name="cardNumber"> Audio card number. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> Linux command line with audio settings. </returns>
        public async Task SetVolumeInDecibels(float decibels, int cardNumber, string controlName = "PCM")
        {
            var currentState = await GetVolumeState().ConfigureAwait(false);

            if (currentState.Decibels != decibels)
                await SetVolumeCommand(cardNumber, $"{decibels}dB", controlName).ConfigureAwait(false);
            else
                return;
        }

        /// <summary>
        /// Mutes or unmutes the current card.
        /// </summary>
        /// <param name="mute"> Object containing new state data. </param>
        /// <param name="cardNumber"> Audio card number. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> Linux command line with audio settings. </returns>
        public async Task ToggleMute(bool mute, int cardNumber, string controlName = "PCM")
        {
            var currentState = await GetVolumeState().ConfigureAwait(false);
            
            if (currentState.IsMute != mute)
                await SetVolumeCommand(cardNumber, mute ? "mute" : "unmute", controlName).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads a command to write to amixer file for volume control.
        /// </summary>
        /// <param name="cardNumber"> Audio card to modify parameters. </param>
        /// <param name="command"> PCM command for amixer device. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> Performs an async write to amixer. </returns>
        private Task<string> SetVolumeCommand(int cardNumber, string command, string controlName) =>
            ProcessRunner.GetProcessOutputAsync("amixer", $"-c {cardNumber} set {controlName} {command}");
    }
}
