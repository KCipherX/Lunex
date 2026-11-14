import { Component, inject, signal, computed, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AccountService } from '../../core/services/account-service';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { AvatarComponent } from '../../shared/components/avatar/avatar.component';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, ButtonComponent, AvatarComponent],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class NavComponent {
  private accountService = inject(AccountService);
  private router = inject(Router);

  mobileMenuOpen = signal(false);
  userMenuOpen = signal(false);
  scrolled = signal(false);

  currentUser = this.accountService.currentUser;
  isAuthenticated = this.accountService.isAuthenticated;

  @HostListener('window:scroll')
  onScroll() {
    this.scrolled.set(window.scrollY > 10);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    if (!target.closest('.user-menu')) {
      this.userMenuOpen.set(false);
    }
  }

  toggleMobileMenu() {
    this.mobileMenuOpen.update(v => !v);
  }

  closeMobileMenu() {
    this.mobileMenuOpen.set(false);
  }

  toggleUserMenu() {
    this.userMenuOpen.update(v => !v);
  }

  async logout() {
    this.accountService.logout();
    this.userMenuOpen.set(false);
    await this.router.navigate(['/']);
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .map(n => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }
}