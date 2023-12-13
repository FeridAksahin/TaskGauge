
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/taskGaugeHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("userJoined", function (username) {
    console.log('User joined: ' + username); 
    debugger;
    $('#participants').append('<li class="participant">' + username + '</li>');
});

connection.on("userList", function (roomUserList) {
    for (var item = 0; item < roomUserList.length; item++) {
        $('#participants').append('<li class="participant">' + roomUserList[item] + '</li>');
    }
});

let urlParams = new URLSearchParams(window.location.search);
let roomNameFromUrl = urlParams.get('roomName'); 

connection.start().then(function () {
    connection.invoke("joinRoom", roomNameFromUrl);
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("userLeft", function (username) {
    $('#participants li:contains("' + username + '")').remove();
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
