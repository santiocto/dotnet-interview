using System.Net.Http.Json;
using System.Net;

namespace TodoMcpServer
{
    public class TodoApiClient
    {
        private readonly HttpClient _httpClient;

        public TodoApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // TodoLists
        public async Task<string> GetTodoListsRawAsync(CancellationToken ct = default)
        {
            using var res = await _httpClient.GetAsync("api/todolists", ct).ConfigureAwait(false);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        }

        public async Task<string?> GetTodoListRawAsync(long id, CancellationToken ct = default)
        {
            using var res = await _httpClient.GetAsync($"api/todolists/{id}", ct).ConfigureAwait(false);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        }

        public async Task<string> CreateTodoListRawAsync(string name, CancellationToken ct = default)
        {
            var payload = new { name };
            using var res = await _httpClient.PostAsJsonAsync("api/todolists", payload, ct).ConfigureAwait(false);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        }

        public async Task<string?> UpdateTodoListRawAsync(long id, string name, CancellationToken ct = default)
        {
            var payload = new { name };
            using var res = await _httpClient.PutAsJsonAsync($"api/todolists/{id}", payload, ct).ConfigureAwait(false);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTodoListAsync(long id, CancellationToken ct = default)
        {
            using var res = await _httpClient.DeleteAsync($"api/todolists/{id}", ct).ConfigureAwait(false);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
            res.EnsureSuccessStatusCode();
            return true;
        }

        // TodoItems
        public async Task<string?> GetTodoItemRawAsync(long todoListId, long itemId, CancellationToken ct = default)
        {
            using var res = await _httpClient.GetAsync($"api/todolists/{todoListId}/items/{itemId}", ct).ConfigureAwait(false);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        }

        public async Task<string?> CreateTodoItemRawAsync(long todoListId, string description, CancellationToken ct = default)
        {
            var payload = new { description };
            using var res = await _httpClient.PostAsJsonAsync($"api/todolists/{todoListId}/items", payload, ct).ConfigureAwait(false);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        }

        public async Task<string?> UpdateTodoItemRawAsync(long todoListId, long itemId, string description, CancellationToken ct = default)
        {
            var payload = new { description };
            using var res = await _httpClient.PutAsJsonAsync($"api/todolists/{todoListId}/items/{itemId}", payload, ct).ConfigureAwait(false);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        }

        public async Task<string?> CompleteTodoItemRawAsync(long todoListId, long itemId, CancellationToken ct = default)
        {
            using var res = await _httpClient.PostAsync($"api/todolists/{todoListId}/items/{itemId}/complete", null, ct).ConfigureAwait(false);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTodoItemAsync(long todoListId, long itemId, CancellationToken ct = default)
        {
            using var res = await _httpClient.DeleteAsync($"api/todolists/{todoListId}/items/{itemId}", ct).ConfigureAwait(false);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
            res.EnsureSuccessStatusCode();
            return true;
        }
    }
}

