# EventLogParser
## Overview
EventLogParser is an application that will parse archived, zipped Windows security logs for event id 4624 for a specific account.  All security event log entries of our domain controllers in our environment are forwarded to another server.  The forwarded logs are then archived and compressed.  I use this application instead of PowerShell due to the performance of PowerShell's event log cmdlets.
