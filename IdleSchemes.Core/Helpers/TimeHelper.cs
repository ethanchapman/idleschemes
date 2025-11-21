using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace IdleSchemes.Core.Helpers {
    public enum TimeFormat {

    }
    public static class TimeHelper {

        public static ImmutableDictionary<string, TimeZoneInfo> TimeZones { get; } = TimeZoneInfo.GetSystemTimeZones()
            .ToImmutableDictionary(tz => tz.Id);

        public static TimeZoneInfo EasternTimeZone { get; } = TimeZones["Eastern Standard Time"];
        public static TimeZoneInfo CentralTimeZone { get; } = TimeZones["Central Standard Time"];
        public static TimeZoneInfo MountainTimeZone { get; } = TimeZones["Mountain Standard Time"];
        public static TimeZoneInfo PacificTimeZone { get; } = TimeZones["Pacific Standard Time"];

        public static string GetDateTimeString(DateTime dateTime, string? timeZoneId, bool compact = false, bool showTime = true) {
            dateTime = ConvertFromUtc(dateTime, timeZoneId);
            if (compact) {
                return dateTime.ToString("MM/dd/yyyy") + (showTime ? " " + GetTimeString(dateTime, null) : "");
            } else {
                return dateTime.ToString("MMM dd, yyyy") + (showTime ? " " + GetTimeString(dateTime, null) : "");
            }
        }

        public static string GetTimeString(DateTime dateTime, string? timeZoneId, bool use24HourClock = false) {
            dateTime = ConvertFromUtc(dateTime, timeZoneId);
            return dateTime.ToString("hh:mmtt").ToLowerInvariant();
        }

        public static string GetReangeString(DateTime start, DateTime end, string? timeZoneId, bool compact = false) {
            start = ConvertFromUtc(start, timeZoneId);
            end = ConvertFromUtc(end, timeZoneId);
            return GetRangeStartString(start, end, null, compact) + " - " + GetRangeEndString(start, end, null, compact);
        }

        public static string GetRangeStartString(DateTime start, DateTime end, string? timeZoneId, bool compact = false) {
            start = ConvertFromUtc(start, timeZoneId);
            end = ConvertFromUtc(end, timeZoneId);
            return GetDateTimeString(start, null, compact: compact);
        }

        public static string GetRangeEndString(DateTime start, DateTime end, string? timeZoneId, bool compact = false) {
            start = ConvertFromUtc(start, timeZoneId);
            end = ConvertFromUtc(end, timeZoneId);
            if(start.Date == end.Date) {
                return GetTimeString(end, null);
            } else {
                return GetDateTimeString(end, null, compact: compact);
            }
        }

        public static DateTime ConvertFromUtc(DateTime utcDateTime, string? timeZoneId) {
            if (timeZoneId is null) {
                return utcDateTime;
            }
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZones[timeZoneId]);
        }

        public static DateTime? ConvertFromUtc(DateTime? utcDateTime, string? timeZoneId) {
            if (utcDateTime is null) {
                return null;
            }
            if (timeZoneId is null) {
                return utcDateTime;
            }
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.Value, TimeZones[timeZoneId]);
        }

        public static DateTime ConvertToUtc(DateTime userDateTime, string? timeZoneId) {
            if (timeZoneId is null) {
                return userDateTime;
            }
            return TimeZoneInfo.ConvertTimeToUtc(userDateTime, TimeZones[timeZoneId]);
        }

        public static DateTime? ConvertToUtc(DateTime? userDateTime, string? timeZoneId) {
            if (userDateTime is null) {
                return null;
            }
            if (timeZoneId is null) {
                return userDateTime;
            }
            return TimeZoneInfo.ConvertTimeToUtc(userDateTime.Value, TimeZones[timeZoneId]);
        }

        public static void SetFromAndToIfNecessary([NotNull] ref DateTime? from, [NotNull] ref DateTime? to, DateTime now, out TimeOrder order, TimeSpan defaultSpan, TimeSpan maxSpan, TimeOrder defaultTimeOrder = TimeOrder.Forward) {
            if (from is not null && to is not null) {
                order = from.Value <= to ? TimeOrder.Forward : TimeOrder.Backward;
            } else {
                order = defaultTimeOrder;
            }

            if(from is null && to is not null) {
                from = order == TimeOrder.Forward ? (to.Value - defaultSpan) : (to.Value + defaultSpan);
            } else {
                from = from ?? now;
                to = order == TimeOrder.Forward ? (from.Value + defaultSpan) : (from.Value - defaultSpan);
            }
        }

        public static IQueryable<T> OrderByTime<T>(this IQueryable<T> queryable, Expression<Func<T, DateTime>> expr, TimeOrder order) {
            if (order is TimeOrder.Forward) {
                return queryable.OrderBy(expr);
            } else {
                return queryable.OrderByDescending(expr);
            }
        }

        public static IQueryable<T> OrderByTime<T>(this IQueryable<T> queryable, Expression<Func<T, DateTime?>> expr, TimeOrder order) {
            if (order is TimeOrder.Forward) {
                return queryable.OrderBy(expr);
            } else {
                return queryable.OrderByDescending(expr);
            }
        }

    }

    public enum TimeOrder {
        Backward,
        Forward
    }
}
