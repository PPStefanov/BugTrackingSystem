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

    // Update toggle button icon based on sidebar state
    function updateToggleIcon(isCollapsed) {
        const icon = toggleBtn.querySelector('i');
        if (icon) {
            if (isCollapsed) {
                icon.className = 'fas fa-bars';
            } else {
                icon.className = 'fas fa-times';
            }
        }
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
    
    // Add smooth transitions after page load
    setTimeout(() => {
        sidebar.style.transition = 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)';
        mainContent.style.transition = 'all 0.3s ease';
        if (footer) footer.style.transition = 'all 0.3s ease';
    }, 100);
});

// Function to set active navigation item based on current URL
function setActiveNavItem() {
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll('.sidebar-nav .nav-link');
    
    navLinks.forEach(link => {
        link.classList.remove('active');
        const href = link.getAttribute('href');
        if (href && currentPath.includes(href.toLowerCase().split('/')[1])) {
            link.classList.add('active');
        }
    });

    // Default to Dashboard if no match
    if (!document.querySelector('.sidebar-nav .nav-link.active')) {
        const dashboardLink = document.querySelector('.sidebar-nav .nav-link[href*="Home"]');
        if (dashboardLink) {
            dashboardLink.classList.add('active');
        }
    }
}

// Smooth scrolling for anchor links
document.addEventListener('click', function(e) {
    if (e.target.matches('a[href^="#"]')) {
        e.preventDefault();
        const target = document.querySelector(e.target.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    }
});
