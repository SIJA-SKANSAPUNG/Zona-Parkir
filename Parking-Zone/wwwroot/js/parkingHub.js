"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/parkingHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Gate event handler
connection.on("ReceiveGateEvent", (event) => {
    console.log("Gate Event:", event);
    // Update UI based on event type
    const eventLog = document.getElementById('gate-event-log');
    if (eventLog) {
        const eventItem = document.createElement('li');
        eventItem.textContent = `${event.eventType} at Gate ${event.gateId} - ${new Date(event.timestamp).toLocaleString()}`;
        eventLog.prepend(eventItem);
    }
});

// Gate status update handler
connection.on("GateStatusUpdated", (status) => {
    console.log("Gate Status Updated:", status);
    // Update UI for gate status
    const gateElement = document.getElementById(`gate-${status.gateId}`);
    if (gateElement) {
        gateElement.innerHTML = `
            <div>Gate: ${status.status.gate}</div>
            <div>Sensor: ${status.status.sensor}</div>
            <div>Last Updated: ${new Date(status.lastUpdated).toLocaleString()}</div>
        `;
    }
});

// Vehicle detection handler
connection.on("VehicleDetected", (data) => {
    console.log("Vehicle Detected:", data);
    // Update UI for vehicle detection
    const detectionLog = document.getElementById('vehicle-detection-log');
    if (detectionLog) {
        const detectionItem = document.createElement('li');
        detectionItem.textContent = `Vehicle detected at Gate ${data.gateId} - ${new Date(data.timestamp).toLocaleString()}`;
        detectionLog.prepend(detectionItem);
    }
});

// Command result handler
connection.on("CommandResult", (result) => {
    console.log("Command Result:", result);
    // Update UI for command result
    const commandLog = document.getElementById('command-log');
    if (commandLog) {
        const commandItem = document.createElement('li');
        commandItem.className = result.success ? 'success' : 'error';
        commandItem.textContent = `${result.command} at Gate ${result.gateId}: ${result.message} - ${new Date(result.timestamp).toLocaleString()}`;
        commandLog.prepend(commandItem);
    }
});

// Plate detection result handler
connection.on("PlateDetectionResult", (result) => {
    console.log("Plate Detection:", result);
    // Update UI for plate detection
    const plateLog = document.getElementById('plate-detection-log');
    if (plateLog) {
        const plateItem = document.createElement('li');
        plateItem.className = result.isSuccessful ? 'success' : 'error';
        plateItem.textContent = `Plate detected: ${result.licensePlate} - ${result.isSuccessful ? 'Success' : 'Failed'}`;
        plateLog.prepend(plateItem);
    }
});

// Barrier status handler
connection.on("BarrierStatusChanged", (status) => {
    console.log("Barrier Status:", status);
    // Update UI for barrier status
    const barrierElement = document.getElementById(`barrier-${status.barrierType.toLowerCase()}`);
    if (barrierElement) {
        barrierElement.className = `barrier-status ${status.isOpen ? 'open' : 'closed'}`;
        barrierElement.textContent = `${status.barrierType} Barrier: ${status.status}`;
    }
});

// Success message handler
connection.on("ShowSuccess", (message) => {
    console.log("Success:", message);
    // Show success message in UI
    showNotification('success', message);
});

// Error message handler
connection.on("ShowError", (message) => {
    console.error("Error:", message);
    // Show error message in UI
    showNotification('error', message);
});

// Receipt printing handler
connection.on("PrintReceipt", (receiptData) => {
    console.log("Printing Receipt:", receiptData);
    // Handle receipt printing
    if (window.printReceipt) {
        window.printReceipt(receiptData);
    }
});

// Vehicle entry handler
connection.on("ReceiveVehicleEntry", (plateNumber) => {
    console.log(`Vehicle ${plateNumber} entered`);
    // Update vehicle entry list
    const entryList = document.getElementById('vehicle-entry-list');
    if (entryList) {
        const entryItem = document.createElement('li');
        entryItem.textContent = `Vehicle ${plateNumber} entered at ${new Date().toLocaleString()}`;
        entryList.prepend(entryItem);
    }
});

// Vehicle exit handler
connection.on("ReceiveVehicleExit", (plateNumber) => {
    console.log(`Vehicle ${plateNumber} exited`);
    // Update vehicle exit list
    const exitList = document.getElementById('vehicle-exit-list');
    if (exitList) {
        const exitItem = document.createElement('li');
        exitItem.textContent = `Vehicle ${plateNumber} exited at ${new Date().toLocaleString()}`;
        exitList.prepend(exitItem);
    }
});

// Transaction update handler
connection.on("TransactionUpdated", (transaction) => {
    console.log('Transaction updated:', transaction);
    // Update transaction list or dashboard
    const transactionList = document.getElementById('transaction-list');
    if (transactionList) {
        const transactionItem = document.createElement('li');
        transactionItem.textContent = `Transaction ${transaction.id}: ${transaction.status} - Amount: ${transaction.amount}`;
        transactionList.prepend(transactionItem);
    }
});

// Occupancy update handler
connection.on("OccupancyUpdated", (occupancyData) => {
    console.log('Parking occupancy updated:', occupancyData);
    // Update occupancy indicators
    const occupancyElement = document.getElementById('parking-occupancy');
    if (occupancyElement) {
        occupancyElement.textContent = `${occupancyData.occupiedSpots}/${occupancyData.totalSpots} (${occupancyData.occupancyRate}%)`;
    }
});

// Gate status update handler
connection.on("ReceiveGateStatus", (gateId, isOpen) => {
    console.log(`Gate ${gateId} status: ${isOpen ? "Open" : "Closed"}`);
    
    // Update UI for gate status
    const gateElement = document.getElementById(`gate-${gateId}`);
    if (gateElement) {
        gateElement.textContent = isOpen ? "Open" : "Closed";
        gateElement.className = `gate-status ${isOpen ? 'open' : 'closed'}`;
    }
});

// Notification helper function
function showNotification(type, message) {
    const notificationContainer = document.getElementById('notification-container') || createNotificationContainer();
    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.textContent = message;
    
    notificationContainer.appendChild(notification);
    setTimeout(() => notification.remove(), 5000);
}

// Create notification container if it doesn't exist
function createNotificationContainer() {
    const container = document.createElement('div');
    container.id = 'notification-container';
    container.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 1000;';
    document.body.appendChild(container);
    return container;
}

// Connection start and error handling
connection.start()
    .then(() => {
        console.log("Connected to ParkingHub");
        // Optional: Notify server of successful connection
        return connection.invoke("OnConnected");
    })
    .catch(err => {
        console.error("SignalR Connection Error:", err);
        // Implement reconnection logic
        setTimeout(() => {
            connection.start()
                .then(() => console.log("Reconnected to ParkingHub"))
                .catch(reconnectErr => console.error("Reconnection failed:", reconnectErr));
        }, 5000);
    });

// Optional: Add method to join specific parking zone
function joinParkingZone(parkingZoneId) {
    connection.invoke("JoinParkingZone", parkingZoneId)
        .then(() => console.log(`Joined parking zone ${parkingZoneId}`))
        .catch(err => console.error(`Error joining parking zone ${parkingZoneId}:`, err));
}

// Optional: Add method to leave specific parking zone
function leaveParkingZone(parkingZoneId) {
    connection.invoke("LeaveParkingZone", parkingZoneId)
        .then(() => console.log(`Left parking zone ${parkingZoneId}`))
        .catch(err => console.error(`Error leaving parking zone ${parkingZoneId}:`, err));
}
