import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { NavComponent } from '../layout/nav/nav';
import { ToastContainerComponent } from '../shared/components/toast/toast-container.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NavComponent, RouterOutlet, ToastContainerComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class AppComponent {
  private router = inject(Router);

  constructor() {
    // App initialization
  }

  get isStandaloneRoute(): boolean {
    const path = this.router.url.split(/[?#]/)[0];
    return ['/', '/login', '/register', '/forgot-password'].includes(path);
  }
}