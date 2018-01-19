# Gen Controller

This project is a fullly-programmable custom Auto-Generator Start (AGS) for Onan Generators.

# Hardware Requirements 

* Raspberry Pi
* DS3231 Real-time clock module
* Pimoroni Automation Hat
* Onan generator cables

# Software Requirements

* Windows 10 IoT Core

# Components

The project is comprised of these pieces:

* [GenController.Uwp](./GenController.Uwp) Universal Windows App, contains the UI screens and Windows-specific components.
* [GenController.Portable](./GenController.Portable) .NET Standard library, contains the platform-independent application logic, viewmodels, and domain-independent helper classes.
* [GenController.Portable.Tests](./GenController.Portable.Tests) Unit tests for the application logic

# Dependencies

The project consumes these dependencies:

* IoTFrosting: For control of the Pimoroni Automation Hat and the DS3231 real-time clock.
* Catnap.Server: To present a web backend