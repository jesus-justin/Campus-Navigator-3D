const ui = {
  apiBase: document.getElementById("apiBase"),
  token: document.getElementById("token"),
  externalId: document.getElementById("externalId"),
  displayName: document.getElementById("displayName"),
  loginBtn: document.getElementById("loginBtn"),
  loginMessage: document.getElementById("loginMessage"),
  fromDate: document.getElementById("fromDate"),
  toDate: document.getElementById("toDate"),
  heatmapType: document.getElementById("heatmapType"),
  refreshInterval: document.getElementById("refreshInterval"),
  applyBtn: document.getElementById("applyBtn"),
  resetFiltersBtn: document.getElementById("resetFiltersBtn"),
  status: document.getElementById("status"),
  eventsTotal: document.getElementById("eventsTotal"),
  sessionsTotal: document.getElementById("sessionsTotal"),
  newUsersTotal: document.getElementById("newUsersTotal"),
  questRunsTotal: document.getElementById("questRunsTotal"),
  totalVisits: document.getElementById("totalVisits"),
  topLocation: document.getElementById("topLocation"),
  questSuccess: document.getElementById("questSuccess"),
  heatmap: document.getElementById("heatmap"),
  pathsTable: document.querySelector("#pathsTable tbody"),
  questChart: document.getElementById("questChart"),
  leaderboardTable: document.querySelector("#leaderboardTable tbody"),
  exportHeatmapBtn: document.getElementById("exportHeatmapBtn"),
  exportPathsBtn: document.getElementById("exportPathsBtn"),
  exportQuestBtn: document.getElementById("exportQuestBtn"),
  lastUpdated: document.getElementById("lastUpdated")
};

const defaults = {
  apiBase: "http://localhost/Campus-Navigator-3D/api",
  token: "",
  externalId: "admin-001",
  displayName: "Admin User"
};

let locationMap = {};
const latestData = {
  heatmap: [],
  paths: [],
  questStats: []
};
let autoRefreshTimer = null;

/**
 * Validates URL format
 * @param {string} url - The URL to validate
 * @returns {boolean} True if valid URL
 */
function isValidUrl(url) {
  try {
    new URL(url);
    return true;
  } catch {
    return false;
  }
}

/**
 * Sanitizes user input to prevent XSS
 * @param {string} str - The string to sanitize
 * @returns {string} Sanitized string
 */
function sanitizeInput(str) {
  const div = document.createElement('div');
  div.textContent = str;
  return div.innerHTML;
}

/**
 * Validates date range
 * @param {string} fromDate - Start date
 * @param {string} toDate - End date
 * @returns {boolean} True if valid range
 */
function isValidDateRange(fromDate, toDate) {
  if (!fromDate || !toDate) return false;
  const from = new Date(fromDate);
  const to = new Date(toDate);
  return from <= to;
}

function setDefaults() {
  ui.apiBase.value = localStorage.getItem("cn_api_base") || defaults.apiBase;
  ui.token.value = localStorage.getItem("cn_token") || defaults.token;
  ui.externalId.value = localStorage.getItem("cn_external_id") || defaults.externalId;
  ui.displayName.value = localStorage.getItem("cn_display_name") || defaults.displayName;

  const today = new Date();
  const prior = new Date();
  prior.setDate(today.getDate() - 7);

  ui.fromDate.value = localStorage.getItem("cn_from_date") || formatDate(prior);
  ui.toDate.value = localStorage.getItem("cn_to_date") || formatDate(today);
  ui.heatmapType.value = localStorage.getItem("cn_heatmap_type") || "building";
  ui.refreshInterval.value = localStorage.getItem("cn_refresh_interval") || "0";
}

async function loginAdmin() {
  const base = ui.apiBase.value.trim() || defaults.apiBase;
  
  // Validate API base URL
  if (!isValidUrl(base)) {
    ui.loginMessage.textContent = "Invalid API base URL";
    return;
  }
  
  const payload = {
    externalId: ui.externalId.value.trim(),
    displayName: ui.displayName.value.trim()
  };

  // Validate required fields
  if (!payload.externalId || !payload.displayName) {
    ui.loginMessage.textContent = "External ID and Display Name are required";
    return;
  }

  ui.loginMessage.textContent = "Requesting token...";

  try {
    const res = await fetch(base.replace(/\/$/, "") + "/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!res.ok) {
      throw new Error(`HTTP ${res.status}`);
    }

    const data = await res.json();
    if (data && data.token) {
      ui.token.value = data.token;
      localStorage.setItem("cn_token", data.token);
    }

    localStorage.setItem("cn_external_id", payload.externalId);
    localStorage.setItem("cn_display_name", payload.displayName);

    if (data.user && data.user.role !== "admin") {
      ui.loginMessage.textContent = "Token issued, but this user is not admin.";
    } else {
      ui.loginMessage.textContent = "Token issued. You can load analytics now.";
    }
  } catch (err) {
    ui.loginMessage.textContent = "Login failed. Check API base and credentials.";
  }
}

function formatDate(date) {
  const y = date.getFullYear();
  const m = String(date.getMonth() + 1).padStart(2, "0");
  const d = String(date.getDate()).padStart(2, "0");
  return `${y}-${m}-${d}`;
}

function dateToRange(dateStr, endOfDay) {
  if (!dateStr) return "";
  return endOfDay ? `${dateStr} 23:59:59` : `${dateStr} 00:00:00`;
}

function setStatus(connected) {
  ui.status.textContent = connected ? "Connected" : "Disconnected";
  ui.status.style.background = connected ? "rgba(71, 201, 177, 0.2)" : "rgba(255, 122, 110, 0.2)";
  ui.status.style.borderColor = connected ? "rgba(71, 201, 177, 0.4)" : "rgba(255, 122, 110, 0.4)";
}

async function fetchJson(path) {
  const base = ui.apiBase.value.trim() || defaults.apiBase;
  const token = ui.token.value.trim();
  const res = await fetch(base.replace(/\/$/, "") + path, {
    headers: token ? { Authorization: `Bearer ${token}` } : {}
  });
  if (!res.ok) {
    throw new Error(`HTTP ${res.status}`);
  }
  return await res.json();
}

async function loadDashboard() {
  const from = dateToRange(ui.fromDate.value, false);
  const to = dateToRange(ui.toDate.value, true);
  const type = ui.heatmapType.value;

  localStorage.setItem("cn_api_base", ui.apiBase.value.trim());
  localStorage.setItem("cn_token", ui.token.value.trim());
  localStorage.setItem("cn_from_date", ui.fromDate.value);
  localStorage.setItem("cn_to_date", ui.toDate.value);
  localStorage.setItem("cn_heatmap_type", ui.heatmapType.value);
  localStorage.setItem("cn_refresh_interval", ui.refreshInterval.value);

  try {
    const locations = await fetchJson("/locations");
    locationMap = buildLocationMap(locations.locations || []);
    const overview = await fetchJson(`/admin/overview?from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}`);
    const heatmap = await fetchJson(`/admin/heatmap?from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}&type=${encodeURIComponent(type)}`);
    const paths = await fetchJson(`/admin/paths?from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}&top=15`);
    const questStats = await fetchJson(`/admin/quest-stats?from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}`);
    const leaderboard = await fetchJson(`/admin/quest-leaderboard?from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}&limit=10`);

    setStatus(true);
    renderOverview(overview.data || {});
    latestData.heatmap = heatmap.data || [];
    latestData.paths = paths.data || [];
    latestData.questStats = questStats.data || [];
    renderHeatmap(latestData.heatmap);
    renderPaths(latestData.paths, locationMap);
    renderQuestStats(latestData.questStats);
    renderLeaderboard(leaderboard.data || []);
    updateLastUpdated(true);
  } catch (err) {
    setStatus(false);
    renderOverview({});
    renderHeatmap([]);
    renderPaths([], {});
    renderQuestStats([]);
    renderLeaderboard([]);
    updateLastUpdated(false);
  }
}

function updateLastUpdated(success) {
  const now = new Date();
  if (success) {
    ui.lastUpdated.textContent = `Last updated: ${now.toLocaleString()}`;
    return;
  }
  ui.lastUpdated.textContent = `Last update failed: ${now.toLocaleString()}`;
}

function setupAutoRefresh() {
  if (autoRefreshTimer) {
    clearInterval(autoRefreshTimer);
    autoRefreshTimer = null;
  }

  const seconds = Number(ui.refreshInterval.value || 0);
  if (!seconds) {
    return;
  }

  autoRefreshTimer = setInterval(() => {
    loadDashboard();
  }, seconds * 1000);
}

function resetFilters() {
  const today = new Date();
  const prior = new Date();
  prior.setDate(today.getDate() - 7);

  ui.fromDate.value = formatDate(prior);
  ui.toDate.value = formatDate(today);
  ui.heatmapType.value = "building";

  localStorage.removeItem("cn_from_date");
  localStorage.removeItem("cn_to_date");
  localStorage.removeItem("cn_heatmap_type");
}

function toCsvValue(value) {
  const text = String(value ?? "");
  const escaped = text.replace(/"/g, '""');
  return `"${escaped}"`;
}

function downloadCsv(filename, rows) {
  if (!rows.length) {
    return;
  }

  const headers = Object.keys(rows[0]);
  const lines = [headers.map(toCsvValue).join(",")];

  rows.forEach((row) => {
    lines.push(headers.map((key) => toCsvValue(row[key])).join(","));
  });

  const blob = new Blob([lines.join("\n")], { type: "text/csv;charset=utf-8;" });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = filename;
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
}

function renderOverview(data) {
  ui.eventsTotal.textContent = Number(data.eventsTotal || 0).toLocaleString();
  ui.sessionsTotal.textContent = Number(data.sessionsTotal || 0).toLocaleString();
  ui.newUsersTotal.textContent = Number(data.newUsersTotal || 0).toLocaleString();
  ui.questRunsTotal.textContent = Number(data.questRunsTotal || 0).toLocaleString();
}

function buildLocationMap(locations) {
  const map = {};
  locations.forEach((loc) => {
    map[loc.id] = loc.name || `Location ${loc.id}`;
  });
  return map;
}

function renderHeatmap(rows) {
  ui.heatmap.innerHTML = "";
  let total = 0;
  let top = "-";
  let topCount = 0;

  rows.forEach((row) => {
    const count = Number(row.count || 0);
    total += count;
    if (count > topCount) {
      topCount = count;
      top = row.name || `Location ${row.location_id}`;
    }
  });

  const max = Math.max(...rows.map(r => Number(r.count || 0)), 1);

  rows.forEach((row) => {
    const item = document.createElement("div");
    item.className = "heatmap-item";
    const label = document.createElement("div");
    label.textContent = row.name || `Location ${row.location_id}`;
    const bar = document.createElement("div");
    bar.className = "heat-bar";
    bar.style.width = `${(Number(row.count || 0) / max) * 100}%`;
    const value = document.createElement("div");
    value.textContent = row.count;

    item.append(label, bar, value);
    ui.heatmap.appendChild(item);
  });

  ui.totalVisits.textContent = total.toLocaleString();
  ui.topLocation.textContent = top;
}

function renderPaths(rows, map) {
  ui.pathsTable.innerHTML = "";
  if (!rows.length) {
    const tr = document.createElement("tr");
    tr.innerHTML = "<td colspan=\"3\" class=\"muted\">No path data</td>";
    ui.pathsTable.appendChild(tr);
    return;
  }

  rows.forEach((row) => {
    const fromName = map[row.from_location_id] || row.from_location_id;
    const toName = map[row.to_location_id] || row.to_location_id;
    const tr = document.createElement("tr");
    tr.innerHTML = `<td>${fromName}</td><td>${toName}</td><td>${row.count}</td>`;
    ui.pathsTable.appendChild(tr);
  });
}

function renderQuestStats(rows) {
  ui.questChart.innerHTML = "";
  let totalRuns = 0;
  let successRuns = 0;

  rows.forEach((row) => {
    const total = Number(row.total_runs || 0);
    const success = Number(row.success_runs || 0);
    totalRuns += total;
    successRuns += success;

    const pct = total > 0 ? Math.round((success / total) * 100) : 0;

    const item = document.createElement("div");
    item.className = "quest-item";

    const title = document.createElement("div");
    title.textContent = row.title || `Quest ${row.quest_id}`;

    const rowInfo = document.createElement("div");
    rowInfo.className = "quest-row";
    rowInfo.innerHTML = `<span>${success}/${total} success</span><span>${Math.round(row.avg_time_sec || 0)} sec avg</span>`;

    const progress = document.createElement("div");
    progress.className = "progress";
    const bar = document.createElement("span");
    bar.style.width = `${pct}%`;
    progress.appendChild(bar);

    item.append(title, rowInfo, progress);
    ui.questChart.appendChild(item);
  });

  const overall = totalRuns > 0 ? Math.round((successRuns / totalRuns) * 100) : 0;
  ui.questSuccess.textContent = `${overall}%`;
}

function renderLeaderboard(rows) {
  ui.leaderboardTable.innerHTML = "";
  if (!rows.length) {
    const tr = document.createElement("tr");
    tr.innerHTML = "<td colspan=\"4\" class=\"muted\">No leaderboard data</td>";
    ui.leaderboardTable.appendChild(tr);
    return;
  }

  rows.forEach((row) => {
    const tr = document.createElement("tr");
    tr.innerHTML = `<td>${row.title || `Quest ${row.quest_id}`}</td><td>${Number(row.runs || 0)}</td><td>${Number(row.success_rate || 0)}%</td><td>${Number(row.avg_time_sec || 0)}</td>`;
    ui.leaderboardTable.appendChild(tr);
  });
}

ui.applyBtn.addEventListener("click", loadDashboard);
ui.loginBtn.addEventListener("click", loginAdmin);
ui.refreshInterval.addEventListener("change", () => {
  localStorage.setItem("cn_refresh_interval", ui.refreshInterval.value);
  setupAutoRefresh();
});
ui.resetFiltersBtn.addEventListener("click", () => {
  resetFilters();
  loadDashboard();
});
ui.exportHeatmapBtn.addEventListener("click", () => downloadCsv("heatmap.csv", latestData.heatmap));
ui.exportPathsBtn.addEventListener("click", () => downloadCsv("paths.csv", latestData.paths));
ui.exportQuestBtn.addEventListener("click", () => downloadCsv("quest-stats.csv", latestData.questStats));

setDefaults();
setupAutoRefresh();
loadDashboard();
