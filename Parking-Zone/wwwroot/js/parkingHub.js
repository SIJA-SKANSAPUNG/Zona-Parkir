"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/parkingHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

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
