import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';

/** Toolbar plus the routed page beneath it. */
@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule
  ],
  template: `
    <mat-toolbar color="primary">
      <span class="main-layout__title">Address Book</span>

      <nav class="main-layout__nav">
        <a mat-button routerLink="/contacts" routerLinkActive="main-layout__link--active">Contacts</a>
        <a mat-button routerLink="/settings" routerLinkActive="main-layout__link--active">Settings</a>
      </nav>

      <span class="main-layout__spacer"></span>

      @if (fullName) {
        <span class="main-layout__user">{{ fullName }}</span>
      }

      <button class="main-layout__logout" mat-button (click)="logout()">
        <mat-icon>logout</mat-icon>
        Logout
      </button>

      <!-- Phone layout: everything above collapses into this menu. -->
      <button
        class="main-layout__menu-button"
        mat-icon-button
        [matMenuTriggerFor]="navMenu"
        aria-label="Open navigation menu"
      >
        <mat-icon>menu</mat-icon>
      </button>
    </mat-toolbar>

    <mat-menu #navMenu="matMenu">
      @if (fullName) {
        <div class="main-layout__menu-user" disabled>{{ fullName }}</div>
      }
      <a mat-menu-item routerLink="/contacts">
        <mat-icon>contacts</mat-icon>
        <span>Contacts</span>
      </a>
      <a mat-menu-item routerLink="/settings">
        <mat-icon>settings</mat-icon>
        <span>Settings</span>
      </a>
      <button mat-menu-item (click)="logout()">
        <mat-icon>logout</mat-icon>
        <span>Logout</span>
      </button>
    </mat-menu>

    <main class="main-layout__content">
      <router-outlet />
    </main>
  `,
  styles: [
    `
      .main-layout__title {
        font-weight: 500;
        margin-right: 24px;
      }

      .main-layout__nav {
        display: flex;
        gap: 4px;
      }

      .main-layout__link--active {
        font-weight: 700;
      }

      .main-layout__spacer {
        flex: 1 1 auto;
      }

      .main-layout__user {
        margin-right: 8px;
        opacity: 0.9;
      }

      .main-layout__content {
        padding: 24px;
      }

      /* The menu button only exists on narrow viewports. */
      .main-layout__menu-button {
        display: none;
      }

      .main-layout__menu-user {
        padding: 8px 16px;
        opacity: 0.6;
        font-size: 0.85rem;
      }

      /* On a phone the toolbar would be wider than the viewport, so the nav, user
         name and logout collapse into a single menu icon. */
      @media (max-width: 599.98px) {
        mat-toolbar {
          padding: 0 8px;
        }

        .main-layout__title {
          margin-right: 8px;
          font-size: 1rem;
        }

        .main-layout__nav,
        .main-layout__user,
        .main-layout__logout {
          display: none;
        }

        .main-layout__menu-button {
          display: inline-flex;
        }

        .main-layout__content {
          padding: 16px;
        }
      }
    `
  ]
})
export class MainLayoutComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly fullName = this.authService.getFullName();

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
