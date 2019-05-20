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
        /// <param name="cardNumber"> card number to get state from. </param>
        /// <param name="deviceName"> Name of the device. </param>
        /// <returns> Volume state object with current volume control settings info. </returns>
        public static async Task<AudioState> GetAudioDeviceState(int cardNumber = 0, string deviceName = "PCM")
        {
            var volumeInfo = await ProcessRunner.GetProcessOutputAsync("amixer", $"-c {cardNumber} get {deviceName}").ConfigureAwait(false);
            var volumeLine = volumeInfo.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => x.Trim().StartsWith("Mono:", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

            var sections = volumeLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var level = int.Parse(sections[3].Substring(1, sections[3].Length - 3), System.Globalization.NumberFormatInfo.InvariantInfo);
            var db = sections[4].Substring(0, sections[4].Length - 2);
            var decibels = float.Parse(db, System.Globalization.NumberFormatInfo.InvariantInfo);
            var isMute = sections[5].Equals("[off]", StringComparison.CurrentCultureIgnoreCase);

            return new AudioState(cardNumber, deviceName, level, decibels, isMute);
        }

        /// <summary>
        /// Sets new volume state.
        /// </summary>
        /// <param name="level"> Object containing new state data. </param>
        /// <param name="cardNumber"> Audio card number. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> Linux command line with audio settings. </returns>
        public async Task<string> SetVolumePercentage(int level, int cardNumber = 0, string controlName = "PCM")
        {
            var result = await SetAudioCommand($"{level}%", cardNumber, controlName).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Increments or decrements device volume by decibels.
        /// </summary>
        /// <param name="decibels"> How many decibels to increment or decrement. </param>
        /// <param name="sign"> Determine if perform an increment or decrement. </param>
        /// <param name="cardNumber"> Sound card number. </param>
        /// <param name="controlName"> controller name. </param>
        /// <returns> A task incrementing or decrementing volume</returns>
        public async Task VolumeByDecibels(float decibels, string sign, int cardNumber = 0, string controlName = "PCM")
        {
            var currentState = await GetAudioDeviceState(cardNumber, controlName).ConfigureAwait(false);
            await SetAudioCommand($"{decibels}{sign}", cardNumber, controlName).ConfigureAwait(false);
        }

        /// <summary>
        /// Mutes or unmutes the current card.
        /// </summary>
        /// <param name="mute"> Object containing new state data. </param>
        /// <param name="cardNumber"> Audio card number. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> Linux command line with audio settings. </returns>
        public async Task ToggleMute(bool mute, int cardNumber = 0, string controlName = "PCM")
        {
            var currentState = await GetAudioDeviceState(cardNumber, controlName).ConfigureAwait(false);
            if (currentState.IsMute != mute)
                await SetAudioCommand(mute ? "mute" : "unmute", cardNumber, controlName).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads a command to write to amixer file for volume control.
        /// </summary>
        /// <param name="command"> PCM command for amixer device. </param>
        /// <param name="cardNumber"> Audio card to modify parameters. </param>
        /// <param name="controlName"> Control name. </param>
        /// <returns> Performs an async write to amixer. </returns>
        private Task<string> SetAudioCommand(string command, int cardNumber = 0, string controlName = "PCM")
        {
            var taskResult = ProcessRunner.GetProcessOutputAsync("amixer", $"-q -c {cardNumber} -- set {controlName} {command}");
            try
            {
                if (!string.IsNullOrWhiteSpace(taskResult.Result))
                    throw new InvalidOperationException(taskResult.Result);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
            }

            return taskResult;
        }
    }
}
