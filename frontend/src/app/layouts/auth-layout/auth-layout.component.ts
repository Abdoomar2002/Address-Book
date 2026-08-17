import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { RouterOutlet } from '@angular/router';

/** Fullscreen, centered card. No navigation - the user is not signed in yet. */
@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [RouterOutlet, MatCardModule],
  template: `
    <div class="auth-layout">
      <mat-card class="auth-layout__card">
        <mat-card-content>
          <router-outlet />
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [
    `
      .auth-layout {
        display: flex;
        align-items: center;
        justify-content: center;
        min-height: 100vh;
        padding: 16px;
        box-sizing: border-box;
        background: var(--mat-app-background-color, #fafafa);
      }

      .auth-layout__card {
        width: 100%;
        max-width: 420px;
      }
    `
  ]
})
export class AuthLayoutComponent {}
