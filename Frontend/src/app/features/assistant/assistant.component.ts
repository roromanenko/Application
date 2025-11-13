import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { AiAssistantResponse, AiQueryRequest, Client } from '../../core/api/generated-api';
import { NotificationService } from '../../core/services/notification.service';

interface Message {
  role: 'user' | 'assistant' | 'system';
  text: string;
  timestamp: Date;
  isError?: boolean;
  data?: any[];
}

@Component({
  selector: 'app-assistant',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './assistant.component.html',
  styleUrl: './assistant.component.scss',
})
export class AssistantComponent implements OnInit, OnDestroy {
  @ViewChild('messageContainer') private messageContainer!: ElementRef;
  @ViewChild('inputField') private inputField!: ElementRef;

  userInput: string = '';
  messages: Message[] = [];
  isLoading = false;
  exampleQueries: string[] = [];

  private destroy$ = new Subject<void>();

  constructor(
    private client: Client,
    private notification: NotificationService
  ) { }

  ngOnInit(): void {
    this.addWelcomeMessage();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private addWelcomeMessage(): void {
    this.messages.push({
      role: 'system',
      text: 'Hello! I\'m your AI assistant. I can help you find information about events, participants, and schedules. Try asking me something!',
      timestamp: new Date(),
    });
  }

  sendMessage(): void {
    const query = this.userInput.trim();
    if (!query || this.isLoading) return;

    this.messages.push({
      role: 'user',
      text: query,
      timestamp: new Date(),
    });

    this.userInput = '';
    this.isLoading = true;
    this.scrollToBottom();

    const request = new AiQueryRequest({
      query
    });

    this.client.query(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: AiAssistantResponse) => {
          if (res.success && res.answer) {
            this.messages.push({
              role: 'assistant',
              text: res.answer,
              timestamp: new Date(),
              data: res.data
            });
          } else if (res.errorMessage) {
            this.messages.push({
              role: 'assistant',
              text: res.errorMessage,
              timestamp: new Date(),
              isError: true
            });
          }
          this.isLoading = false;
          this.scrollToBottom();
        },
        error: (err: any) => {
          console.error('AI request failed', err);

          const errorMessage = err.error?.detail
            || err.error?.title
            || 'Unable to connect to AI service. Please try again later.';

          this.messages.push({
            role: 'assistant',
            text: errorMessage,
            timestamp: new Date(),
            isError: true
          });

          this.notification.error(errorMessage);
          this.isLoading = false;
          this.scrollToBottom();
        }
      });
  }

  useExampleQuery(query: string): void {
    this.userInput = query;
    this.inputField?.nativeElement?.focus();
  }

  clearChat(): void {
    this.messages = [];
    this.addWelcomeMessage();
  }

  onKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      if (this.messageContainer) {
        this.messageContainer.nativeElement.scrollTop =
          this.messageContainer.nativeElement.scrollHeight;
      }
    }, 100);
  }

  formatTime(date: Date): string {
    return new Intl.DateTimeFormat('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    }).format(date);
  }

  copyToClipboard(text: string): void {
    navigator.clipboard.writeText(text).then(() => {
      this.notification.success('Text copied to clipboard');
    }).catch(err => {
      console.error('Failed to copy text', err);
      this.notification.error('Failed to copy text');
    });
  }

  hasStructuredData(message: Message): boolean {
    return !!(message.data && Array.isArray(message.data) && message.data.length > 0);
  }

  getTableHeaders(data: any[]): string[] {
    if (!data || data.length === 0) return [];
    return Object.keys(data[0]);
  }

  formatCellValue(value: any): string {
    if (value === null || value === undefined) return '';
    if (value instanceof Date) return this.formatDate(value);
    if (typeof value === 'boolean') return value ? 'Yes' : 'No';
    return String(value);
  }

  private formatDate(date: Date): string {
    return new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    }).format(new Date(date));
  }
}
