export type CardType = "feature" | "bug" | "chore" | "urgent";
export type Priority = 1 | 2 | 3;

export type Card = {
  id: string;
  columnId: string;
  title: string;
  description?: string;
  type: CardType;
  priority: Priority;
  dueAt?: string;
  order: number;
  status: "Backlog" | "Ready" | "Doing" | "Done";
};

export type Column = {
  id: string;
  name: string;
  order: number;
  cards: Card[];
};

export type Board = {
  id: string;
  quarterId: string;
  columns: Column[];
};
