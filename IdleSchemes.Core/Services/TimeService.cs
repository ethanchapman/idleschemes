using IdleSchemes.Core.Helpers;

namespace IdleSchemes.Core.Services {
    public class TimeService {
        public DateTime GetNow() {
            return DateTime.UtcNow;
        }

        public DateTime GetNow(string timeZoneId) {
            return TimeHelper.ConvertFromUtc(DateTime.UtcNow, timeZoneId);
        }

    }
}
