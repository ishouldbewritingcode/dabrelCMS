using Microsoft.AspNetCore.Http.Features;

namespace dabrelCMS.code
{
	public class Calendar
	{
namespace dabrelCMS.code
{
	public class Calendar
	{

		public string GenerateMonthCalendar(int? month = null, int? year = null, List<CMSItem>? items = null)
		{
			// Use current date if parameters not provided
			DateTime today = DateTime.Now;
			int selectedMonth = month ?? today.Month;
			int selectedYear = year ?? today.Year;

			// Validate month is in range
			if (selectedMonth < 1 || selectedMonth > 12)
				throw new ArgumentException("Month must be between 1 and 12", nameof(month));

			// Get first day of month and total days in month
			DateTime firstDay = new DateTime(selectedYear, selectedMonth, 1);
			int daysInMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);
			int startingDayOfWeek = (int)firstDay.DayOfWeek; // 0 = Sunday

			// Build HTML
			var html = new System.Text.StringBuilder();
			html.AppendLine($"<div class=\"calendar calendar-{selectedYear}-{selectedMonth}\">");
			html.AppendLine($"<h2 class=\"calendar-header\">{firstDay:MMMM yyyy}</h2>");
			html.AppendLine("<div class=\"calendar-grid\">");

			// Day headers
			foreach (var day in new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" })
			{
				html.AppendLine($"<div class=\"calendar-day-header\">{day}</div>");
			}

			// Empty cells before month starts
			for (int i = 0; i < startingDayOfWeek; i++)
			{
				html.AppendLine("<div class=\"calendar-day calendar-day-empty\"></div>");
			}

			// Days of month
			for (int day = 1; day <= daysInMonth; day++)
			{
				DateTime cellDate = new DateTime(selectedYear, selectedMonth, day);
				bool isToday = cellDate.Date == today.Date;
				bool isWeekend = cellDate.DayOfWeek == DayOfWeek.Saturday || cellDate.DayOfWeek == DayOfWeek.Sunday;

				string classes = "calendar-day";
				if (isToday) classes += " calendar-day-today";
				if (isWeekend) classes += " calendar-day-weekend";

				html.AppendLine($"<div class=\"{classes}\" data-date=\"{cellDate:yyyy-MM-dd}\">");
				html.AppendLine($"<span class=\"calendar-day-number\">{day}</span>");
				// need to add items for the day if provided
				if (items != null)
				{
					var dayItems = items
							.Where(i => i.Start.HasValue && i.Start.Value.Date == cellDate.Date)
							.ToList();
					foreach (var item in dayItems)
					{
						html.AppendLine($"<div class=\"calendar-item\">{item.Title1}</div>");
					}
				}
				html.AppendLine("</div>");
			}

			// Fill remaining cells
			int lastCellIndex = startingDayOfWeek + daysInMonth - 1;
			int remainingCells = 6 * 7 - (lastCellIndex + 1); // 6 rows * 7 days
			for (int i = 0; i < remainingCells; i++)
			{
				html.AppendLine("<div class=\"calendar-day calendar-day-empty\"></div>");
			}

			html.AppendLine("</div>");
			html.AppendLine("</div>");

			return html.ToString();
		}
	}
}	

	}
}
