# E-sports-turneringsplatform

## Gruppe
### Natasja Vitoft : cph-nn194@cphbusiness.dk
### Lasse Baggesgård Hansen : cph-lh479@cphbusiness.dk

Application is made as a simple CLI interface, for interacting with the PostgreSQL database.
It is written in C#, and we use Npgsql library as database connector, which is similar to functionality as JDBC in Java.

On starting the program the user is greeted with:

1. Register Player
2. Join Tournament
3. Submit Match Result
4. Submit Match Result pessimistically
5. Update tournament date optimistically
6. Pessimistic stress test Match result update
7. Optimistic stress test set tournament start date
8. Pessimistic Register player to tournament
9. Optimistic stress test register player to tournament
10. Update tournament date pessimistically
11. Pessimistic stress test update tournament date<br>
q. Quit

The user can then choose a number to run the desired operation, after which the program will display the choices again


## Stress test results:

__Test Scenario:__<br>
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

When not queueing the transactions, the execution time is significantly smaller, but only 3 transactions are succesful, indicating that 47 of the transactions wa blocked by the data being locked by another transaction.

### Optimistic Concurrency Control (PCC) Results

| Metric | Value|
|----------|----------|
| Execution Time (ms)   | 00:00:00.0843369   |
| Number of successful transactions  | 1  |
| Rollbacks  | 49  |


### Comparison table

| **Metric**               | **Optimistic CC (OCC)**                                                                 | **Pessimistic CC (PCC)**                                                            |
|-------------------------|----------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------|
| **Execution Time**       | OCC is much faster than PCC, as shown in our tests.                                    | PCC is much slower because it has significant wait time (unless using the `NO WAIT` keyword in SQL). |
| **Transaction Success Rate** | OCC has a lower transaction success rate because it only checks for conflicts at the commit stage and does not lock rows during execution. | PCC has a higher transaction success rate because it locks the row at the start of the transaction, preventing conflicts and reducing rollbacks at the commit stage. |
| **Lock Contention**      | Minimal locking.                                                                       | High locking due to rows being locked during the transaction.                      |
| **Best Use Case**        | OCC is best suited for use cases where conflicts are not expected to occur frequently, such as in systems that are primarily read-heavy. An example of this could be **Wikipedia**, where reading is much more common than updating, which happens less often. If updates to a page do occur simultaneously, they typically don't cause serious conflicts, and a message like *“Try again”* would not be critical. | PCC is ideal for use cases with frequent updates. An example is a **ticketing system**, where a row representing a ticket should be locked when someone is in the process of purchasing it. It would be problematic if a user enters all their purchase details only to receive an error at the end, stating that the ticket is no longer available. |
