// Sidebar Toggle Functionality
document.addEventListener('DOMContentLoaded', function() {
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('main-content');
    const footer = document.querySelector('.footer');
    const toggleBtn = document.getElementById('sidebar-toggle');
    
    // Check if elements exist
    if (!sidebar || !mainContent || !toggleBtn) {
        console.warn('Sidebar elements not found');
        return;
    }

    // Load saved sidebar state
    const sidebarCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
    if (sidebarCollapsed) {
        sidebar.classList.add('collapsed');
        mainContent.classList.add('expanded');
        if (footer) footer.classList.add('expanded');
        updateToggleIcon(true);
    }

    // Toggle sidebar on button click
    toggleBtn.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        
        const isCurrentlyCollapsed = sidebar.classList.contains('collapsed');
        
        if (window.innerWidth <= 768) {
            // Mobile behavior - show/hide sidebar
            sidebar.classList.toggle('show');
        } else {
            // Desktop behavior - collapse/expand sidebar
            sidebar.classList.toggle('collapsed');
            mainContent.classList.toggle('expanded');
            if (footer) footer.classList.toggle('expanded');
            
            // Update toggle icon
            updateToggleIcon(!isCurrentlyCollapsed);
            
            // Save state to localStorage
            localStorage.setItem('sidebarCollapsed', !isCurrentlyCollapsed);
        }
        
        // Add ripple effect
        addRippleEffect(toggleBtn, e);
    });

    // Toggle button always shows hamburger menu - no icon switching needed
    function updateToggleIcon(isCollapsed) {
        // Icon stays the same - always hamburger menu
        // This function is kept for consistency but doesn't change the icon
    }

    // Add ripple effect to button
    function addRippleEffect(button, event) {
        const ripple = document.createElement('span');
        const rect = button.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = event.clientX - rect.left - size / 2;
        const y = event.clientY - rect.top - size / 2;
        
        ripple.style.cssText = `
            position: absolute;
            width: ${size}px;
            height: ${size}px;
            left: ${x}px;
            top: ${y}px;
            background: rgba(255, 255, 255, 0.3);
            border-radius: 50%;
            transform: scale(0);
            animation: ripple 0.6s ease-out;
            pointer-events: none;
        `;
        
        button.appendChild(ripple);
        
        setTimeout(() => {
            ripple.remove();
        }, 600);
    }

    // Handle responsive behavior
    function handleResize() {
        if (window.innerWidth <= 768) {
            // Mobile behavior
            sidebar.classList.remove('collapsed', 'show');
            mainContent.classList.remove('expanded');
            if (footer) footer.classList.remove('expanded');
            updateToggleIcon(false);
        } else {
            // Desktop behavior - restore saved state
            const sidebarCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
            if (sidebarCollapsed) {
                sidebar.classList.add('collapsed');
                mainContent.classList.add('expanded');
                if (footer) footer.classList.add('expanded');
                updateToggleIcon(true);
            } else {
                sidebar.classList.remove('collapsed');
                mainContent.classList.remove('expanded');
                if (footer) footer.classList.remove('expanded');
                updateToggleIcon(false);
            }
        }
    }

    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function(e) {
        if (window.innerWidth <= 768 && 
            sidebar.classList.contains('show') &&
            !sidebar.contains(e.target) && 
            !toggleBtn.contains(e.target)) {
            sidebar.classList.remove('show');
        }
    });

    // Handle window resize
    window.addEventListener('resize', handleResize);

    // Set active navigation item
    setActiveNavItem();
    
    // Refresh active navigation on page navigation (for SPA-like behavior)
    window.addEventListener('popstate', setActiveNavItem);
    
    // Add smooth transitions after page load
    setTimeout(() => {
        sidebar.style.transition = 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)';
        mainContent.style.transition = 'all 0.3s ease';
        if (footer) footer.style.transition = 'all 0.3s ease';
        
        // Refresh navigation highlighting after transitions are set
        setActiveNavItem();
    }, 100);
});

// Function to set active navigation item based on current URL
function setActiveNavItem() {
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll('.sidebar-nav .nav-link');
    
    // Remove active class from all links first
    navLinks.forEach(link => {
        link.classList.remove('active');
    });
    
    // Define specific path mappings for exact matching
    const pathMappings = [
        { paths: ['/bugreport/list', '/bugreport/details', '/bugreport/edit'], selector: 'a[href*="/BugReport/List"]' },
        { paths: ['/bugreport/form', '/bugreport/create'], selector: 'a[href*="/BugReport/Form"]' },
        { paths: ['/applications/list', '/applications/create', '/applications/edit'], selector: 'a[href*="/Applications/List"]' },
        { paths: ['/user/list', '/user/create', '/user/edit'], selector: 'a[href*="/User/List"]' },
        { paths: ['/reports', '/reports/index', '/reports/analytics', '/reports/timebased', '/reports/userproductivity', '/reports/applicationanalysis'], selector: 'a[href*="/Reports/Index"]' },
        { paths: ['/', '/home', '/home/index'], selector: 'a[href*="/Home/Index"]' }
    ];
    
    // Find the best match
    let activeLink = null;
    for (const mapping of pathMappings) {
        if (mapping.paths.some(path => currentPath === path || currentPath.startsWith(path + '/'))) {
            activeLink = document.querySelector(mapping.selector);
            break;
        }
    }
    
    // If we found a match, set it as active
    if (activeLink) {
        activeLink.classList.add('active');
    } else {
        // Default to Dashboard for root path or unknown paths
        const dashboardLink = document.querySelector('a[href*="/Home/Index"]');
        if (dashboardLink) {
            dashboardLink.classList.add('active');
        }
    }
}

// Handle navigation link clicks
document.addEventListener('click', function(e) {
    // Handle anchor links
    if (e.target.matches('a[href^="#"]')) {
        e.preventDefault();
        const target = document.querySelector(e.target.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
        return;
    }
    
    // Handle navigation links
    const navLink = e.target.closest('.sidebar-nav .nav-link');
    if (navLink) {
        // Remove active class from all navigation links
        document.querySelectorAll('.sidebar-nav .nav-link').forEach(link => {
            link.classList.remove('active');
        });
        
        // Add active class to clicked link
        navLink.classList.add('active');
        
        // Close sidebar on mobile after navigation
        if (window.innerWidth <= 768) {
            const sidebar = document.getElementById('sidebar');
            if (sidebar) {
                sidebar.classList.remove('show');
            }
        }
    }
});

// Refresh navigation highlighting when page changes (for back/forward navigation)
window.addEventListener('pageshow', function() {
    setTimeout(setActiveNavItem, 50);
});

// AJAX Notification System
document.addEventListener('DOMContentLoaded', function() {
    initializeNotifications();
});

function initializeNotifications() {
    const notificationBtn = document.getElementById('notification-btn');
    const notificationDropdown = document.getElementById('notification-dropdown');
    const notificationBadge = document.getElementById('notification-badge');
    
    if (!notificationBtn || !notificationDropdown || !notificationBadge) {
        return; // Elements not found, user might not be logged in
    }

    // Load initial notifications
    loadNotifications();
    
    // Set up periodic refresh (every 30 seconds)
    setInterval(loadNotifications, 30000);
    
    // Handle notification button click
    notificationBtn.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        toggleNotificationDropdown();
    });
    
    // Close dropdown when clicking outside
    document.addEventListener('click', function(e) {
        if (!notificationBtn.contains(e.target) && !notificationDropdown.contains(e.target)) {
            hideNotificationDropdown();
        }
    });
}

function loadNotifications() {
    fetch('/api/NotificationsApi', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
        credentials: 'same-origin'
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Failed to load notifications');
        }
        return response.json();
    })
    .then(notifications => {
        updateNotificationBadge(notifications);
        renderNotifications(notifications);
    })
    .catch(error => {
        console.error('Error loading notifications:', error);
        showNotificationError();
    });
}

function updateNotificationBadge(notifications) {
    const badge = document.getElementById('notification-badge');
    const unreadCount = notifications.filter(n => !n.isRead).length;
    
    if (unreadCount > 0) {
        badge.textContent = unreadCount > 99 ? '99+' : unreadCount.toString();
        badge.style.display = 'flex';
    } else {
        badge.style.display = 'none';
    }
}

function renderNotifications(notifications) {
    const dropdown = document.getElementById('notification-dropdown');
    const unreadCount = notifications.filter(n => !n.isRead).length;
    
    let html = `
        <div class="notification-header">
            <h6>Notifications</h6>
            <span class="notification-count">${unreadCount}</span>
        </div>
        <div class="notification-body">
    `;
    
    if (notifications.length === 0) {
        html += `
            <div class="no-notifications">
                <i class="fas fa-bell-slash"></i>
                <p>No notifications yet</p>
            </div>
        `;
    } else {
        notifications.forEach(notification => {
            const iconClass = getNotificationIcon(notification.type);
            const isUnread = !notification.isRead ? 'unread' : '';
            
            html += `
                <div class="notification-item ${isUnread}" data-id="${notification.id}">
                    <div class="notification-content">
                        <div class="notification-icon ${notification.type.toLowerCase()}">
                            <i class="${iconClass}"></i>
                        </div>
                        <div class="notification-text">
                            <div class="notification-title">${escapeHtml(notification.title)}</div>
                            <div class="notification-message">${escapeHtml(notification.message)}</div>
                            <div class="notification-meta">
                                <span class="notification-time">${notification.timeAgo}</span>
                                <div class="notification-actions">
                                    ${!notification.isRead ? `<button class="notification-action-btn mark-read" onclick="markNotificationAsRead(${notification.id})">Mark as read</button>` : ''}
                                </div>
                            </div>
                            ${notification.url ? `<a href="${notification.url}" class="notification-link">View Details</a>` : ''}
                        </div>
                    </div>
                </div>
            `;
        });
    }
    
    html += `
        </div>
        <div class="notification-footer">
            ${unreadCount > 0 ? `<button class="mark-all-read-btn" onclick="markAllNotificationsAsRead()">Mark all as read</button>` : ''}
        </div>
    `;
    
    dropdown.innerHTML = html;
}

function getNotificationIcon(type) {
    switch (type.toLowerCase()) {
        case 'bugreport':
        case 'bug':
            return 'fas fa-bug';
        case 'comment':
            return 'fas fa-comment';
        case 'assignment':
            return 'fas fa-user-tag';
        case 'status':
            return 'fas fa-info-circle';
        default:
            return 'fas fa-bell';
    }
}

function toggleNotificationDropdown() {
    const dropdown = document.getElementById('notification-dropdown');
    const isVisible = dropdown.style.display === 'block';
    
    if (isVisible) {
        hideNotificationDropdown();
    } else {
        showNotificationDropdown();
    }
}

function showNotificationDropdown() {
    const dropdown = document.getElementById('notification-dropdown');
    dropdown.style.display = 'block';
    dropdown.classList.add('show');
    
    // Load fresh notifications when opening
    loadNotifications();
}

function hideNotificationDropdown() {
    const dropdown = document.getElementById('notification-dropdown');
    dropdown.style.display = 'none';
    dropdown.classList.remove('show');
}

function markNotificationAsRead(notificationId) {
    fetch(`/api/NotificationsApi/${notificationId}/mark-read`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
        credentials: 'same-origin'
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Failed to mark notification as read');
        }
        return response.json();
    })
    .then(result => {
        if (result.success) {
            // Reload notifications to update UI
            loadNotifications();
        }
    })
    .catch(error => {
        console.error('Error marking notification as read:', error);
    });
}

function markAllNotificationsAsRead() {
    fetch('/api/NotificationsApi/mark-all-read', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
        credentials: 'same-origin'
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Failed to mark all notifications as read');
        }
        return response.json();
    })
    .then(result => {
        if (result.success) {
            // Reload notifications to update UI
            loadNotifications();
        }
    })
    .catch(error => {
        console.error('Error marking all notifications as read:', error);
    });
}

function showNotificationError() {
    const dropdown = document.getElementById('notification-dropdown');
    dropdown.innerHTML = `
        <div class="notification-error">
            <i class="fas fa-exclamation-triangle"></i>
            <p>Failed to load notifications</p>
            <button onclick="loadNotifications()" class="btn btn-sm btn-primary">Try Again</button>
        </div>
    `;
}

function getAntiForgeryToken() {
    const token = document.querySelector('input[name="__RequestVerificationToken"]');
    return token ? token.value : '';
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
