/*----------------------------------------------------------------------------*/
/* Unity Utilities                                                            */
/* Copyright (C) 2022 Manifold Games - All Rights Reserved                    */
/*                                                                            */
/* Unauthorized copying of this file, via any medium is strictly prohibited   */
/* Proprietary and confidential                                               */
/* Do NOT release into public domain                                          */
/*                                                                            */
/* The above copyright notice and this permission notice shall be included in */
/* all copies or substantial portions of the Software.                        */
/*                                                                            */
/* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR */
/* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,   */
/* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL    */
/* THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER */
/* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING    */
/* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER        */
/* DEALINGS IN THE SOFTWARE.                                                  */
/*                                                                            */
/* Written by Manifold Games <hello.manifoldgames@gmail.com>                  */
/*----------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A stack (First In - Last Out) collection with read-only access to elements.
/// </summary>
public class StackArray<T> : IEnumerable<T>, IReadOnlyCollection<T>
{
  public StackArray(int capacity = 1)
  {
    _list = new List<T>(capacity);
  }
  
  private List<T> _list;

  public int Capacity
  {
    get => _list.Capacity;
    set => _list.Capacity = value;
  }
  
  public int Count     => _list.Count;
  public T this[int i] => _list[i];

  public T Pop()
  {
    T value = Peek();
    _list.RemoveAt(Count - 1);
    return value;
  }

  public T Peek()
  {
    // NOTE(WSWhitehouse): Throwing exception if the stack array is empty! Cannot return an "invalid value" as we 
    // don't know what counts as invalid (null is a valid value that the StackArray can hold, so we can't use it).
    // This is also the default behaviour in the built-in Stack and Array class.
    if (Count <= 0) throw new InvalidOperationException("The StackArray is empty!");
    return _list[Count - 1];
  }
  
  public void Push(T value) => _list.Add(value);
  
  public void Clear()           => _list.Clear();
  public bool Contains(T value) => _list.Contains(value);

  public IEnumerator<T> GetEnumerator()
  {
    foreach (T value in _list)
    {
      yield return value;
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
