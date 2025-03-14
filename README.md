# E-sports-turneringsplatform

## Gruppe
### Natasja Vitoft : cph-nn194@cphbusiness.dk
### Lasse Baggesgård Hansen : cph-lh479@cphbusiness.dk

Application is made as a simple CLI interface, for interacting with the PostgreSQL database.
It is written in C#, and we use Npgsql library as database connector, which is similar to functionality as JDBC in Java.

On starting the program the user is greeted with:

1. Register Player
2. Join Tournament
3. Submit Match Result<br>
q. Quit


## Stress test results:

__Test Scenario:__
The scenario used is a function for updating start dates on a tournament concurrently. we created 2 different transaction management styles for this function, namely optimistic and pessimistic to compare the results.
We use 50 concurrent threads for testing the scenario

### 2️⃣ Pessimistic Concurrency Control (PCC) Results

This method was tested with 2 different locking methods; the `FOR UPDATE`, for high rate of succesful transactions on the cost of execution time and `FOR UPDATE NOWAIT` For observing how many transsactions fail, because they are trying to access locked data.


`FOR UPDATE`
| Metric | Value|
|----------|----------|
| Execution Time (ms)   | 00:00:00.2205228   |
| Number of successful transactions  | 50  |
| Rollbacks  | 0  |

All transactions are successful, but on the cost of a high execution time of 2 millisenconds.

`FOR UPDATE NOWAIT`
| Metric | Value|
|----------|----------|
| Execution Time (ms)   |  00:00:00.0498196  |
| Number of successful transactions  | 3  |
| Rollbacks  | 47  |

When not queueing the transactions, the execution time is significantly smaller, but only 3 transactions are succesful, indicating that 47 of the transactions had to initially way for other transactions to finish.