
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

function addTask() {
    let taskName = $('#taskName').val();

    if (taskName.trim() !== "" && !isExistTaskName(taskName)) {
        connection.invoke("addTask", taskName);
    }
}

connection.on("addedTaskByAdmin", function (addTaskModel) {
    debugger;
    if (addTaskModel.isSuccess) {
        let tableRow =
            `<tr>
            <td>${addTaskModel.taskName}</td>
            <td>Continues</td>
            <td><button type="button" class="btn btn-primary btn-sm" onclick="openTaskModal(${addTaskModel.taskName})">Detail</button></td>
            <td><button type="button" class="btn btn-primary btn-sm" onclick="openTaskModal(${addTaskModel.taskName})">Delete</button></td>
            </tr>`;

        $('#taskHistoryTable').append(tableRow);
    }
    else {
        Swal.fire({
            text: addTaskModel.message,
            icon: 'error',
            confirmButtonText: 'OK'
        })
    }

});

function isExistTaskName(taskName) {
    return false;
}
