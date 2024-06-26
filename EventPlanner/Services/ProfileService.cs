using EventPlanner.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using EventPlanner.Models.User;
using EventPlanner.Models.Events;
using EventPlanner.Models.Profile;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace EventPlanner.Services
{
    public class ProfileService : IProfileService
	{
		private IDbContext _context { get; }
		private IImageService _imageService { get; }

		public ProfileService(IDbContext context, IImageService imageService)
		{
			_context = context;
			_imageService = imageService;
		}

		public EventListEntryVM MapEventToListEntry(Event fetchedEvent)
		{
			var eventListEntry = new EventListEntryVM() {
				Id = fetchedEvent.Id,
				AuthorId = fetchedEvent.AuthorId,
				Name = fetchedEvent.Name,
				DateTime = fetchedEvent.DateTime,
				CityName = fetchedEvent.Location.Street.City.Name,
				CategoryName = fetchedEvent.Category.Name,
				EventTypeName = fetchedEvent.EventType.Name,
			};

			return eventListEntry;
		}

		public IEnumerable<EventListEntryVM> MapAllEventsToVM(IEnumerable<Event> fetchedEvents)
		{
			var eventList = new List<EventListEntryVM>();

			foreach (var fetchedEvent in fetchedEvents)
			{
				eventList.Add(MapEventToListEntry(fetchedEvent));
			}

			return eventList;
		}

		public async Task<IEnumerable<Event>> GetUserEvents(int userId, bool showOnlyMyEvents)
		{
			if (showOnlyMyEvents)
			{
				var eventIds = await _context.Event
				.Where(e => e.AuthorId == userId)
				.Select(e => e.Id)
				.ToListAsync();

				var events = await _context.Event
					.Where(e => eventIds.Contains(e.Id))
					.Include(e => e.Location)
						.ThenInclude(l => l.Street)
							.ThenInclude(s => s.City)
					.Include(e => e.Author)
					.Include(e => e.Category)
					.Include(e => e.EventType)
					.ToListAsync();

				return events;
			}
			else
			{
				var retrivedUser = await _context.Users
				.Where(u => u.Id == userId)
				.Include(u => u.Events)
					.ThenInclude(e => e.Location)
						.ThenInclude(l => l.Street)
							.ThenInclude(s => s.City)
				.Include(u => u.Events)
					.ThenInclude(e => e.Author)
				.Include(u => u.Events)
					.ThenInclude(e => e.Category)
				.Include(u => u.Events)
					.ThenInclude(e => e.EventType)
				.AsNoTracking().FirstOrDefaultAsync();

				return retrivedUser.Events;
			}
		}

		public IEnumerable<EventListEntryVM> SortUserEvents(IEnumerable<EventListEntryVM> fetchedEvents, string sortCriteria)
		{
			switch(sortCriteria)
			{
				case "Date":
					return fetchedEvents.OrderBy(e => e.DateTime);
				case "Category":
					return fetchedEvents.OrderBy(e => e.CategoryName);
				case "Type":
					return fetchedEvents.OrderBy(e => e.EventTypeName);
				default:
					return fetchedEvents;
			}
		}

		public async Task<IEnumerable<EventListEntryVM>> GetEventsForView(int userId, string sortCriteria, bool showOnlyMyEvents)
		{
			IEnumerable<Event> fetchedEvents = await GetUserEvents(userId, showOnlyMyEvents);
			IEnumerable<EventListEntryVM> eventsForView = MapAllEventsToVM(fetchedEvents);
			IEnumerable <EventListEntryVM> sortedEventsForView = SortUserEvents(eventsForView, sortCriteria);

			return sortedEventsForView;
		}

		public int GetTotalPages(int eventsNo, int eventsPerPage)
		{
			return (int)Math.Ceiling(eventsNo / (double)eventsPerPage);
		}

		public IEnumerable<EventListEntryVM> GetEventsForPage(IEnumerable<EventListEntryVM> allEvents, int pageNo, int eventsPerPage)
		{
			return allEvents.Skip((pageNo - 1) * eventsPerPage).Take(eventsPerPage);
		}

		public EventsListViewModel ConstructEventsListVM(int userId, IEnumerable<EventListEntryVM> paginatedEvents, int currPageNo, int totalPages, string sortCriteria, bool showOnlyMyEvents)
		{
			var model = new EventsListViewModel()
			{
				UserId = userId,
				Events = paginatedEvents,
				CurrentPage = currPageNo,
				TotalPages = totalPages
			};

			SetModelSortCriteria(ref model, sortCriteria, showOnlyMyEvents);

			return model;
		}

		public SelectList PopulateSortDropdownList(string sortCriteriaSelected = "")
		{
			return new SelectList(new List<SelectListItem>
			{
				new SelectListItem { Text = "Date", Value = "Date" },
				new SelectListItem { Text = "Category", Value = "Category" },
				new SelectListItem { Text = "Type", Value = "Type" }
			}, "Value", "Text", sortCriteriaSelected);
		}

		public EventsListViewModel SetModelSortCriteria(ref EventsListViewModel model, string sortCriteria, bool showOnlyMyEvents)
		{
			model.SortCriteria = PopulateSortDropdownList(sortCriteria);
			model.SortCriteriaSelected = sortCriteria;
			model.ShowOnlyMyEvents = showOnlyMyEvents;

			return model;
		}

		public async Task<EventsListViewModel> GetEventsForCurrentPage(int userId, int pageNo, string sortCriteria, bool showOnlyMyEvents)
		{
			const int EVENTS_PER_PAGE = 10;

			IEnumerable<EventListEntryVM> events = await GetEventsForView(userId, sortCriteria, showOnlyMyEvents);
			int totalEvents = events.Count();
			int totalPages = GetTotalPages(totalEvents, EVENTS_PER_PAGE);
			IEnumerable<EventListEntryVM> paginatedEvents = GetEventsForPage(events, pageNo, EVENTS_PER_PAGE);

			return ConstructEventsListVM(userId, paginatedEvents, pageNo, totalPages, sortCriteria, showOnlyMyEvents);
		}

        public LoginHistoryVM MapToLoginHistoryVM(LoginHistory fetchedEntry)
        {
            return new LoginHistoryVM
            {
                LoginTime = fetchedEntry.LoginTime,
                IPAddress = fetchedEntry.IPAddress,
                DeviceInformation = fetchedEntry.DeviceInformation,
                BrowserInformation = fetchedEntry.BrowserInformation,
                LoginSuccess = fetchedEntry.LoginSuccess,
                FailureReason = fetchedEntry.FailureReason,
            };
        }

		public async Task<IEnumerable<LoginHistory>> GetLoginHistories(int userId, int numberOfLastEntries)
		{
			return await _context.LoginHistory.Where(lh => lh.User.Id == userId).OrderByDescending((e) => e.LoginTime).Skip(0).Take(numberOfLastEntries).ToListAsync();
        }

		public IEnumerable<LoginHistoryVM> MapLoginHistoriesToVM(IEnumerable<LoginHistory> fetchedLoginHistoryList)
		{
            List<LoginHistoryVM> loginHistoryListVM = new();
            foreach (var loginHistory in fetchedLoginHistoryList)
            {
                loginHistoryListVM.Add(MapToLoginHistoryVM(loginHistory));
            }

			return loginHistoryListVM;
        }

		public async Task<IEnumerable<LoginHistoryVM>> GetLoginHistoriesVM(int userId, int numberOfLastEntries)
		{
			IEnumerable<LoginHistory> fetchedLoginHistories = await GetLoginHistories(userId, numberOfLastEntries);
			IEnumerable<LoginHistoryVM> loginHistoriesVM = MapLoginHistoriesToVM(fetchedLoginHistories);

			return loginHistoriesVM;
		}

		public async Task<AppUser> GetUserWithAccountSettings(int userId)
		{
			return await _context.Users.Include(u => u.AccountSettings).Where(u => u.Id == userId).FirstOrDefaultAsync();
		}

		public async Task<SettingsVM> GetSettingsPageVM(int userId)
		{
			const int MAX_LOGIN_HISTORIES = 10;
			IEnumerable<LoginHistoryVM> loginHistoriesVM = await GetLoginHistoriesVM(userId, MAX_LOGIN_HISTORIES);
			AppUser? user = await GetUserWithAccountSettings(userId);
			bool? accountIsHidden = null;

			if (user != null && user.AccountSettings != null)
			{
				accountIsHidden = user.AccountSettings.AccountHidden;
			}

			return new SettingsVM {
				AccountHidden = accountIsHidden,
				LoginHistory = loginHistoriesVM,
			};
		}

		public async Task ToggleAccountVisibility(int userId)
		{
			var userLoaded = await _context.Users.Include(u => u.AccountSettings).Where(u => u.Id == userId).FirstOrDefaultAsync();

			userLoaded.AccountSettings.AccountHidden = !userLoaded.AccountSettings.AccountHidden;
			await _context.SaveChangesAsync();
		}

		public async Task DeleteUserPicture(AppUser user)
		{
            _imageService.DeleteImage(user.ProfileImageUrl!);

			user.ProfileImageUrl = null;

			_context.Attach<AppUser>(user);
			_context.Entry(user).Property("ProfileImageUrl").IsModified = true;
			await _context.SaveChangesAsync();
        }
	}
}