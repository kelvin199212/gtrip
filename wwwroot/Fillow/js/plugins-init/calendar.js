"use strict"

function fullCalender(){
	
	
	/* initialize the external events
		-----------------------------------------------------------------*/

		
	/* initialize the calendar
	-----------------------------------------------------------------*/
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();

    today = yyyy + '-' + mm + '-' + dd;
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        headerToolbar: {
            left: 'title,prev,next',
            right: 'today',
            center: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        selectable: true,
        selectMirror: true,
        select: function (arg) {
            var title = prompt('Event Title:');
            if (title) {
                calendar.addEvent({
                    title: title,
                    start: arg.start,
                    end: arg.end,
                    allDay: arg.allDay
                })
            }
            calendar.unselect()
        },
        editable: true,
        droppable: true, // this allows things to be dropped onto the calendar
        drop: function (arg) {
            // is the "remove after drop" checkbox checked?
            if (document.getElementById('drop-remove').checked) {
                // if so, remove the element from the "Draggable Events" list
                arg.draggedEl.parentNode.removeChild(arg.draggedEl);
            }
        },
        initialDate: today,
        weekNumbers: true,
        navLinks: true, // can click day/week names to navigate views
        editable: true,
        selectable: true,
        nowIndicator: true,
        views: {
            settimana: {
                type: 'agendaWeek',
                duration: {
                    days: 7
                },
                title: 'Apertura',
                columnFormat: 'dddd', // Format the day to only show like 'Monday'
                hiddenDays: [0, 6] // Hide Sunday and Saturday?
            }
        },
        defaultView: 'settimana',
        events: [
            {
                title: 'All Day Event',
                start: '2024-08-19'
            },
            {
                title: 'Annual Meeting Envatos Community with Kleon Team',
                start: '2023-02-07',
                end: '2023-02-10',
                className: "bg-danger"
            },
            {
                groupId: 999,
                title: 'Repeating Event',
                start: '2023-02-09T16:00:00'
            },
            {
                groupId: 999,
                title: 'Repeating Event',
                start: '2024-08-02T16:00:00'
            },
            {
                title: 'Conference',
                start: '2024-08-11',
                end: '2024-08-20',
                className: "bg-danger"
            },
            {
                title: 'Lunch',
                start: '2023-02-12T12:00:00'
            },
            {
                title: 'Meeting',
                start: '2023-04-12T14:30:00'
            },
            {
                title: 'Happy Hour',
                start: '2023-07-12T17:30:00'
            },
            {
                title: 'Dinner',
                start: '2023-02-12T20:00:00',
                className: "bg-warning"
            },
            {
                title: 'Birthday Party',
                start: '2024-08-13T07:00:00',
                className: "bg-secondary",
                description: 'This is the description for Event 1'
            },
            {
                title: 'Click for Google',
                url: 'http://google.com/',
                start: '2023-02-28'
            }
        ],
        eventDidMount: function (info) {
            if (info.event.extendedProps.description) {
                $(info.el).tooltip({
                    title: info.event.extendedProps.description,
                    placement: 'top',
                    trigger: 'hover',
                    container: 'body'
                });
            }
        }
    });
    calendar.render();

}	
	
jQuery(window).on('load',function(){
	setTimeout(function(){
		fullCalender();	
	}, 1000);
	
	
});	
	

		