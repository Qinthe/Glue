import { onBeforeUnmount } from 'vue'
import { ref } from 'vue'

function normalizeKey(key: string) {
  const lower = key.toLowerCase()

  if (lower === 'control') return 'ctrl'
  if (lower === ' ') return 'space'
  if (lower === 'spacebar') return 'space'
  if (lower === 'escape') return 'esc'
  if (lower === 'arrowup') return 'up'
  if (lower === 'arrowdown') return 'down'
  if (lower === 'arrowleft') return 'left'
  if (lower === 'arrowright') return 'right'

  return lower
}

function sortHotkeyKeys(keys: string[]) {
  const priority = ['ctrl', 'alt', 'shift', 'meta']

  return [...keys].sort((left, right) => {
    const leftIndex = priority.indexOf(left)
    const rightIndex = priority.indexOf(right)

    if (leftIndex !== -1 && rightIndex !== -1) return leftIndex - rightIndex
    if (leftIndex !== -1) return -1
    if (rightIndex !== -1) return 1

    return left.localeCompare(right)
  })
}

function toComboString(keys: Iterable<string>) {
  return sortHotkeyKeys(Array.from(keys)).join('+')
}

export function useHotkey(combo: string, handler: () => void) {
  const pressedKeys = new Set<string>()

  const onKeydown = (event: KeyboardEvent) => {
    const key = normalizeKey(event.key)
    pressedKeys.add(key)

    const currentCombo = toComboString(pressedKeys)
    if (currentCombo === combo.toLowerCase()) {
      event.preventDefault()
      handler()
    }
  }

  const onKeyup = (event: KeyboardEvent) => {
    const key = normalizeKey(event.key)
    pressedKeys.delete(key)
  }

  const onBlur = () => {
    pressedKeys.clear()
  }

  window.addEventListener('keydown', onKeydown)
  window.addEventListener('keyup', onKeyup)
  window.addEventListener('blur', onBlur)

  onBeforeUnmount(() => {
    window.removeEventListener('keydown', onKeydown)
    window.removeEventListener('keyup', onKeyup)
    window.removeEventListener('blur', onBlur)
  })
}

export function useHotkeyRecorder(initialValue = '') {
  const value = ref(initialValue)
  const displayValue = ref(initialValue)
  const recording = ref(false)
  const pressedKeys = new Set<string>()

  function start() {
    recording.value = true
    pressedKeys.clear()
    displayValue.value = value.value
  }

  function stop() {
    if (pressedKeys.size > 0) {
      value.value = toComboString(pressedKeys)
      displayValue.value = value.value
    } else {
      displayValue.value = value.value
    }

    recording.value = false
    pressedKeys.clear()
  }

  const onKeydown = (event: KeyboardEvent) => {
    if (!recording.value) return

    event.preventDefault()
    event.stopPropagation()

    if (event.key === 'Escape') {
      recording.value = false
      pressedKeys.clear()
      displayValue.value = value.value
      return
    }

    const key = normalizeKey(event.key)
    pressedKeys.add(key)
    displayValue.value = toComboString(pressedKeys)
  }

  const onKeyup = () => {
    if (!recording.value) return

    if (pressedKeys.size > 0) {
      value.value = toComboString(pressedKeys)
      displayValue.value = value.value
    }
  }

  const onBlur = () => {
    if (!recording.value) return
    stop()
  }

  window.addEventListener('keydown', onKeydown)
  window.addEventListener('keyup', onKeyup)
  window.addEventListener('blur', onBlur)

  onBeforeUnmount(() => {
    window.removeEventListener('keydown', onKeydown)
    window.removeEventListener('keyup', onKeyup)
    window.removeEventListener('blur', onBlur)
  })

  return {
    value,
    displayValue,
    recording,
    start,
    stop,
  }
}