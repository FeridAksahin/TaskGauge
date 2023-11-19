
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/taskGaugeHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("userJoined", function (connectionId) {
    console.log('User joined: ' + connectionId); 
    debugger;
    $('#participants').append('<li class="participant">' + connectionId + '</li>');
});

connection.start().then(function () {
    connection.invoke("joinRoom", "room1");
}).catch(function (err) {
    return console.error(err.toString());
});

function addTask() {
    let taskName = $('#taskName').val();
    if (taskName.trim() !== "") {
        let listItem = '<li class="task">' +
            '<span>' + taskName + '</span>' +
            '<button type="button" class="btn btn-primary btn-sm" onclick="openTaskModal()">Set Duration</button>' +
            '</li>';
        $('#taskList').append(listItem);
        $('#taskName').val("");
    }
}

function openTaskModal() {
    $('#taskModal').modal('show');
}

function setTaskDuration() {
    let taskDuration = $('#taskDuration').val();
    let selectedTask = $('.task.active');
    selectedTask.find('.task-duration').text(' - ' + taskDuration + ' point(s)');
    $('#taskModal').modal('hide');
}

function updateParticipants(participantList) {
    $('#participants').empty();
    participantList.forEach(function (participant) {
        $('#participants').append('<li class="participant">' + participant + '</li>');
    });
} 
