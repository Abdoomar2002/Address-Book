import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';

/** Toolbar plus the routed page beneath it. */
@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatToolbarModule, MatButtonModule, MatIconModule],
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

      <button mat-button (click)="logout()">
        <mat-icon>logout</mat-icon>
        <span class="main-layout__logout-label">Logout</span>
      </button>
    </mat-toolbar>

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

      /* On a phone the toolbar would otherwise be wider than the viewport and
         scroll the whole page sideways. Drop the labels, keep the controls. */
      @media (max-width: 599px) {
        mat-toolbar {
          padding: 0 8px;
        }

        .main-layout__title {
          margin-right: 8px;
          font-size: 1rem;
        }

        .main-layout__nav a {
          min-width: 0;
          padding: 0 8px;
        }

        .main-layout__user,
        .main-layout__logout-label {
          display: none;
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
