# .NET Backend Learning Checklist

状态说明：已完成、进行中、待学习。根据之前的学习进度整理。

## Phase 1 — Backend Fundamentals & Architecture

Request Pipeline & Middleware

Dependency Injection：Transient / Scoped / Singleton

Task、Thread、Async / Await

IEnumerable vs IQueryable

Global Exception Handling & ProblemDetails

Clean Architecture：API / Application / Domain / Infrastructure

CQRS + MediatR、Repository、Unit of Work

## Phase 2 — Database & EF Core Performance

EF Core Tracking vs No-Tracking

Include vs Projection

N+1 Query Problem

SaveChanges & Batch Operations

B-Tree Index、Index Scan、Index Condition

Slow Query & EXPLAIN ANALYZE

OFFSET Pagination vs Keyset Pagination

Cursor Pagination：Base64URL、PageSize + 1

Composite Cursor：CreatedAt + Id

PostgreSQL pg_trgm：优化 ILIKE 搜索

Optimistic Concurrency

Version、ETag、If-Match、409 Conflict、428 Precondition Required

## Phase 3 — HTTP Resilience & Distributed Systems

HttpClientFactory & Typed HttpClient

Sequential I/O vs Task.WhenAll

Timeout、CancellationToken、504 Gateway Timeout

Retry、Exponential Backoff、Jitter

Idempotency-Key + PostgreSQL Unique Constraint

Database Transaction & Unit of Work

Circuit Breaker：Closed → Open → Half-Open

503 Service Unavailable + ProblemDetails

## Phase 4 — Observability & Monitoring

OpenTelemetry Distributed Tracing

Custom ActivitySource & Application Spans

PostgreSQL / HttpClient Traces

Jaeger Trace Analysis

Prometheus Metrics & /metrics

Grafana Dashboard：Request Rate、P50、P95、P99

API 5xx Error Rate

Grafana Alert Rule：High 5xx Rate

备注：SMTP Email Alert 尚未配置；你之前决定暂时不研究 Email Alerting。

## Phase 5 — Caching

1\. IMemoryCache

当前进度

刚开始：AddMemoryCache、Cache Hit/Miss、TTL、Cache Invalidation。还未完成实际 API 实验。

Redis Fundamentals

IDistributedCache + Redis

IMemoryCache vs Redis

HybridCache（.NET 9+）

Cache Invalidation、Stale Data、Cache Stampede

Cache 与 PostgreSQL 的性能比较

## Phase 6 — Runtime Performance & Profiling

CLR、JIT、Garbage Collection（GC）

Allocation、Memory Pressure、Large Object Heap

ThreadPool、ThreadPool Starvation

CPU Profiling、Memory Profiling

.NET Diagnostic Tools：dotnet-counters、dotnet-trace

找到 Hot Path，而不是盲目优化

Load Testing：k6、Throughput、Latency、P95/P99

## Phase 7 — Docker、Kubernetes & Deployment

Docker Image、Container、Volume、Bind Mount

Docker Compose + PostgreSQL

Kubernetes：Pod、Service、Deployment

CI/CD Pipeline

Production Configuration、Health Checks
