# Isoqube Orchestration Framework

## Introduction
Isoqube is an orchestration framework that manages the coordination, execution, and communication between microservices within a distributed architecture. It ensures seamless collaboration between individual services to achieve a common business goal.

Built on top of MassTransit, Isoqube leverages a developer-focused platform for creating distributed applications without complexity. It is highly configurable and supports integration with various storage solutions. Currently, it is configured to work with MongoDB and RabbitMQ.

## Key Features
- Simple attribute-based event and consumer creation.
- Automatic wiring of consumers and events during app bootstrap.
- Flexible and reusable to meet diverse business needs.

## Solution Overview
This repository contains a .NET 8 solution simulating a build server. It includes a configurator application (Isoqube.Controlcenter) that allows users to create and run build configurations. The consumer logic is simulated, providing a framework to implement custom logic and application flows.

## How It Works
1. **Bootstrap Process:** Isoqube registers all consumers to handle events.
2. **Event Handling:** When an event is published, the corresponding consumer processes it and triggers the next event in sequence.
3. **Configurator Application:** Users can view registered events, create run definitions with names and descriptions, and manage configurations from the orchestration tab.
4. **Monitoring:** During runtime, users can observe event consumption through console logs. The RunEntity collection monitors the status of each run.

## Running the Solution
1. Build the solution.
2. Run all projects using your IDE’s Run/Debug configuration.
3. Access the configurator app to view registered topics/events.
4. Create a run definition/configuration.
5. Execute or re-run the configuration from the orchestration tab.
6. Observe event consumption through console logs and monitor the run status via the RunEntity collection.

## Note
This solution is under continuous development. Feel free to report or suggest any issues.

