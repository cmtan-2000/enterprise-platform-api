## IEnumrable vs IQueryable

### IQueryable - Query can still be translated/executed by the data source.

### IEnumerable - Data is already available in memory / enumerable sequence

## What is deferred execution in EF Core?

### "With IQueryable, the query is built but not executed immediately. EF Core executes it when a terminal operation such as ToListAsync, FirstAsync or CountAsync is called. This allows us to compose filters, sorting and pagination before sending the final SQL to the database."

## Why is calling ToListAsync too early a performance problem?

### "Because it materializes the query immediately. Any filtering or pagination applied afterwards happens in application memory instead of the database, which can result in unnecessary data transfer and higher memory usage."

## Why shouldn't we call SaveChangesAsync inside a loop?

### "Because each SaveChangesAsync can result in a database round trip and transaction overhead. For multiple entities, I would normally accumulate the changes and save them as a batch, unless there is a specific transactional or memory reason to commit incrementally."

## Would you always call SaveChanges only once?

### "Not always. For very large datasets, I would process records in batches to control memory usage, transaction size and change-tracking overhead."

## What can cause database connection pool exhaustion?

### "Common causes include long-running queries, long-lived transactions, excessive concurrency, connection leaks, or a database that is overloaded. I would first investigate connection usage and query duration rather than simply increasing the pool size or timeout."

## What's the difference between connection timeout and command timeout?

### "Connection timeout happens while waiting to obtain a database connection. Command timeout happens after a connection has been established and the SQL command takes too long to complete."
